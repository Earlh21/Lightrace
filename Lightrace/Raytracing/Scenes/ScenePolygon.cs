using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Cpu;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Scenes
{
    public class ScenePolygon
    {
        public Material Material { get; }
        public List<Vec2> Points { get; }

        private Lazy<bool> is_clockwise_lazy;

        public bool IsClockwise => is_clockwise_lazy.Value;

        public ScenePolygon(IEnumerable<Vec2> points, Material material)
        {
            Material = material;
            Points = points.ToList();

            is_clockwise_lazy = new(() =>
            {
                int min_y_i = 0;
                float min_y = Points[0].Y;

                for (int i = 1; i < Points.Count; i++)
                {
                    if (Points[i].Y < min_y)
                    {
                        min_y_i = i;
                        min_y = Points[i].Y;
                    }
                }

                int a_index = min_y_i - 1;

                if(a_index < 0)
                {
                    a_index += Points.Count;
                }

                Vec2 ab = Points[a_index] - Points[min_y_i];
                Vec2 ac = Points[(a_index + 1) % Points.Count] - Points[min_y_i];

                return ab.Cross(ac) > 0;
            });
        }

        public ScenePolygon Translated(Vec2 displacement)
        {
            return new ScenePolygon(Points.Select(point => point + displacement).ToList(), Material);
        }

        public ScenePolygon Rotated(float angle, Vec2 origin)
        {
            return new ScenePolygon(Points.Select(point => (point - origin).Rotate(angle) + origin).ToList(), Material);
        }

        public ScenePolygon Rotated(float angle, float x_origin = 0, float y_origin = 0)
        {
            return Rotated(angle, new Vec2(x_origin, y_origin));
        }

        public ScenePolygon Scaled(float scale, Vec2 origin)
        {
            return new ScenePolygon(Points.Select(point => (point - origin) * scale + origin).ToList(), Material);
        }

        public ScenePolygon Scaled(float scale, float x_origin = 0, float y_origin = 0)
        {
            return Scaled(scale, new Vec2(x_origin, y_origin));
        }

        public ScenePolygon Translated(float x, float y)
        {
            return Translated(new Vec2(x, y));
        }

        public Edge[] GetEdges()
        {
            var edges = new Edge[Points.Count];

            for(int i = 0; i < Points.Count - 1; i++)
            {
                edges[i] = new Edge(Points[i], Points[i + 1], IsClockwise);
            }

            edges[Points.Count - 1] = new Edge(Points[^1], Points[0], IsClockwise);

            return edges;
        }

        public static ScenePolygon CreateBox(float width, float height, Material material)
        {
            var points = new List<Vec2>();

            points.Add(new Vec2(-width / 2, -height / 2));
            points.Add(new Vec2(width / 2, -height / 2));
            points.Add(new Vec2(width / 2, height / 2));
            points.Add(new Vec2(-width / 2, height / 2));

            return new ScenePolygon(points, material);
        }

        public static ScenePolygon CreateRegularPolygon(int n, float radius, Material material)
        {
            var points = new List<Vec2>();

            for(int i = 0; i < n; i++)
            {
                float angle = MathF.PI * 2 / n * i;
                points.Add(Vec2.GetDir(angle) * radius);
            }

            return new ScenePolygon(points, material);
        }

        public static ScenePolygon CreateConvex(int point_count, float height, float concavity, Material material)
        {
            float radius = concavity;
            float y = height / 2;

            if (radius - y < 0)
            {
                throw new ArgumentException("Radius must be at least twice height.");
            }

            float x = (float)Math.Sqrt(radius * radius - y * y);

            float start_angle = (new Vec2(x, y)).AngleBetween(new Vec2(1, 0));
            float arc = start_angle * 2;

            var points = new List<Vec2>();

            for(int i = 0; i < point_count; i++)
            {
                var angle = start_angle - i * arc / point_count;
                points.Add(new Vec2(-x, 0) + Vec2.GetDir(angle) * radius);
            }

            for(int i = 0; i < point_count; i++)
            {
                var angle = (float)Math.PI + start_angle - i * arc / point_count;
                points.Add(new Vec2(x, 0) + Vec2.GetDir(angle) * radius);
            }

            return new ScenePolygon(points, material);
        }
    }
}
