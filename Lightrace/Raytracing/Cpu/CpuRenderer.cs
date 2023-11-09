using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Scenes;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    public class CpuRenderer : Renderer
    {
        private GLDrawer drawer;

        //TODO: Dedicated thread per CpuRenderer object for keeping the gl context, drawing, and sending back pixels
        public CpuRenderer(RenderSettings settings) : base(settings)
        {
            drawer = new(settings);
        }

        public override async Task<Vec3[,]> RenderAsync(Scene scene)
        {
            var world = new World(scene);

            var pass_count = 12;// Environment.ProcessorCount;
            var tasks = new List<Task<float[]>>();

            for (int i = 0; i < pass_count; i++)
            {
                var n = i;
                tasks.Add(Task.Run(() => GetLines(world, RenderSettings.SampleCount / pass_count, pass_count)));
            }

            var passes = await Task.WhenAll(tasks);

            //While drawing lines, compute the direct lighting
            var result = new Vec3[RenderSettings.Resolution.X, RenderSettings.Resolution.Y];
            var direct_lighting_task = Task.Run(() => AddDirectLighting(world, result));

            foreach(var pass in passes)
            {
                drawer.QueueLines(pass);
            }

            var traced_lighting = await drawer.GetPixels();

            await direct_lighting_task;

            for (int x = 0; x < RenderSettings.Resolution.X; x++)
            {
                for (int y = 0; y < RenderSettings.Resolution.Y; y++)
                {
                    result[x, y] += traced_lighting[x, y];
                }
            }

            ClampLighting(result);

            Console.WriteLine("CPU done");
            Console.Out.Flush();
            return result;
        }

        private float[] GetLines(World world, int sample_count, int pass_count)
        {
            var lines = new List<float>();

            var scene_boundary = ScenePolygon.CreateBox(RenderSettings.Camera.Size.X, RenderSettings.Camera.Size.Y, new());
            var boundary = new Polygon(scene_boundary.GetEdges(), scene_boundary.Material, -1);

            for (int i = 0; i < sample_count; i++)
            {
                LightSample(world, lines, 1.0f / (sample_count * pass_count), boundary);
            }

            return lines.ToArray();
        }

        private Vec3[,] RenderPass(World world, int sample_count, int pass_count)
        {
            var result = new Vec3[RenderSettings.Resolution.X, RenderSettings.Resolution.Y];

            var scene_boundary = ScenePolygon.CreateBox(RenderSettings.Camera.Size.X, RenderSettings.Camera.Size.Y, new());
            var boundary = new Polygon(scene_boundary.GetEdges(), scene_boundary.Material, -1);

            for (int i = 0; i < sample_count; i++)
            {
                LightSample(world, result, 1.0f / (sample_count * pass_count), boundary);
            }

            return result;
        }

        private void ClampLighting(Vec3[,] image)
        {
            for(int i = 0; i < image.GetLength(0); i++)
            {
                for(int j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = image[i, j].Min(new Vec3(1, 1, 1));
                }
            }
        }

        private void AddDirectLighting(World world, Vec3[,] image)
        {
            for (int x = 0; x < RenderSettings.Resolution.X; x++)
            {
                for (int y = 0; y < RenderSettings.Resolution.Y; y++)
                {
                    var world_pos = new Vec2(x, y) / RenderSettings.ImageScale + RenderSettings.Camera.Center - RenderSettings.Camera.Size / 2;
                    image[x, y] += world.GetDirectedLight(world_pos);
                }
            }
        }

        private void LightSample(World world, List<float> lines, float sample_factor, in Polygon boundary)
        {
            float ray_color_factor = sample_factor * world.TotalIllumination * RenderSettings.ImageScale;

            var rand = new Random();
            bool from_directed = false;
            var ray = world.SampleLightRays(rand, ref from_directed);

            while (ray.Bounces + ray.Transmissions <= RenderSettings.MaxBounces)
            {
                bool hit = false;
                var rec = world.GetWorldHit(ray, ref hit);
                var (new_ray, end_color) = rec.Transform(ray, rand);
                Vec2 line_end_point = new_ray.Origin;

                //Draw to the edge of the image if no object was hit
                if (!hit)
                {
                    bool temp = false;
                    rec = boundary.GetHit(ray, ref temp);
                    line_end_point = rec.Position;

                    if (!temp)
                    {
                        break;
                    }
                }

                if (!from_directed || ray.Bounces + ray.Transmissions > 0)
                {
                    var start_color = ray.Color * ray_color_factor;
                    end_color *= ray_color_factor;

                    lines.Add(ray.Origin.X);
                    lines.Add(ray.Origin.Y);
                    lines.Add(0);
                    lines.Add(start_color.X);
                    lines.Add(start_color.Y);
                    lines.Add(start_color.Z);

                    lines.Add(line_end_point.X);
                    lines.Add(line_end_point.Y);
                    lines.Add(0);
                    lines.Add(end_color.X);
                    lines.Add(end_color.Y);
                    lines.Add(end_color.Z);
                }

                if (!hit)
                {
                    break;
                }

                if (rec.Material.Type == 1)
                {
                    break;
                }

                ray = new_ray;
            }
        }

        private void LightSample(World world, Vec3[,] image, float sample_factor, in Polygon boundary)
        {
            var rand = new Random();
            bool from_directed = false;
            var ray = world.SampleLightRays(rand, ref from_directed);

            while (ray.Bounces + ray.Transmissions <= RenderSettings.MaxBounces)
            {
                bool hit = false;
                var rec = world.GetWorldHit(ray, ref hit);

                //Draw to the edge of the image if no object was hit, then skip transforms
                if(!hit)
                {
                    bool temp = false;
                    rec = boundary.GetHit(ray, ref temp);

                    if(!temp)
                    {
                        break;
                    }
                }

                //We already got the direct light from directed sources, skip drawing non-transformed light rays from them
                if (from_directed && ray.Bounces + ray.Transmissions > 0)
                {
                    Xiaolin(image, ray.Color * sample_factor * world.TotalIllumination * RenderSettings.ImageScale, ray.Origin, rec.Position);
                }

                if (!hit)
                {
                    break;
                }

                if(rec.Material.Type == 1)
                {
                    break;
                }

                (ray, _) = rec.Transform(ray, rand);
            }
        }

        

        private void Xiaolin(Vec3[,] image, Vec3 color, Vec2 start, Vec2 end)
        {
            start = (start - RenderSettings.Camera.Center + RenderSettings.Camera.Size / 2) * RenderSettings.ImageScale;
            end = (end - RenderSettings.Camera.Center + RenderSettings.Camera.Size / 2) * RenderSettings.ImageScale;

            float x0 = start.X;
            float y0 = start.Y;
            float x1 = end.X;
            float y1 = end.Y;

            void Plot(int x, int y, float c)
            {
                if (x >= 0 && y >= 0 && x < image.GetLength(0) && y < image.GetLength(1))
                {
                    image[x, y] += color * c;
                }
            }

            int Round(float x)
            {
                return (int)(x + 0.5f);
            }

            float FPart(float x)
            {
                return x - (int)x;
            }

            float RFPart(float x)
            {
                return 1 - FPart(x);
            }

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;

            float gradient = dx == 0 ? 1 : dy / dx;

            int xend = Round(x0);
            float yend = y0 + gradient * (xend - x0);
            float xgap = RFPart(x0 + 0.5f);
            int xpxl1 = xend;
            int ypxl1 = (int)yend;

            if (steep)
            {
                Plot(ypxl1, xpxl1, RFPart(yend) * xgap);
                Plot(xpxl1, ypxl1 + 1, FPart(yend) * xgap);
            }
            else
            {
                Plot(xpxl1, ypxl1, RFPart(yend) * xgap);
                Plot(xpxl1, ypxl1 + 1, FPart(yend) * xgap);
            }

            float intery = yend + gradient;

            xend = Round(x1);
            yend = y1 + gradient * (xend - x1);
            xgap = FPart(x1 + 0.5f);
            int xpxl2 = xend;
            int ypxl2 = (int)yend;

            if (steep)
            {
                Plot(ypxl2, xpxl2, RFPart(yend) * xgap);
                Plot(ypxl2 + 1, xpxl2, FPart(yend) * xgap);
            }
            else
            {
                Plot(xpxl2, ypxl2, RFPart(yend) * xgap);
                Plot(xpxl2, ypxl2 + 1, FPart(yend) * xgap);
            }

            if (steep)
            {
                for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
                {
                    Plot((int)intery, x, RFPart(intery));
                    Plot((int)intery + 1, x, FPart(intery));
                    intery += gradient;
                }
            }
            else
            {
                for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++)
                {
                    Plot(x, (int)intery, RFPart(intery));
                    Plot(x, (int)intery + 1, FPart(intery));
                    intery += gradient;
                }
            }
        }

        private void Bresenham(Vec3[,] image, Vec3 color, Vec2 start, Vec2 end)
        {
            Vec2i a = (Vec2i)((start - RenderSettings.Camera.Center + RenderSettings.Camera.Size / 2) * RenderSettings.ImageScale + 0.5f);
            Vec2i b = (Vec2i)((end - RenderSettings.Camera.Center + RenderSettings.Camera.Size / 2) * RenderSettings.ImageScale + 0.5f);

            int dx = Math.Abs(b.X - a.X);
            int sx = a.X < b.X ? 1 : -1;
            int dy = -Math.Abs(b.Y - a.Y);
            int sy = a.Y < b.Y ? 1 : -1;
            int error = dx + dy;

            int x = a.X;
            int y = a.Y;

            while (true)
            {
                if (x < image.GetLength(0) && y < image.GetLength(1) && x >= 0 && y >= 0)
                {
                    image[x, y] += color;
                }

                if (x == b.X && y == b.Y)
                {
                    break;
                }

                int e2 = 2 * error;

                if (e2 >= dy)
                {
                    if (x == b.X)
                    {
                        break;
                    }

                    error += dy;

                    x += sx;
                }

                if (e2 < dx)
                {
                    if (y == b.Y)
                    {
                        break;
                    }

                    error += dx;

                    y += sy;
                }
            }
        }
    }
}
