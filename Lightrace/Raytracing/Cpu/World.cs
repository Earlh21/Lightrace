using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Scenes;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    internal class World
    {
        public IReadOnlyCollection<Polygon> Polygons;
        public IReadOnlyCollection<Circle> Circles;
        public IReadOnlyCollection<LightSource> LightSources;

        public float TotalIllumination { get; }

        public World(Scene scene)
        {
            int id = 0;

            Polygons = scene.Polygons.Select(polygon => new Polygon(polygon.GetEdges(), polygon.Material, id++)).ToArray();
            Circles = scene.Circles.Select(circle => new Circle(circle.Position, circle.Radius, circle.Material, id++)).ToArray();

            var light_sources = new List<LightSource>();

            light_sources.AddRange(Polygons.Select(p => (LightSource)p));
            light_sources.AddRange(Circles.Select(c => (LightSource)c));
            light_sources.AddRange(scene.PointLights.Select(l => (LightSource)new PointLight(l.Position, l.Color)));
            light_sources.AddRange(scene.ConeLights.Select(l => (LightSource)new ConeLight(l.Position, l.Direction, l.Angle, l.Color)));
            light_sources.AddRange(scene.EdgeLights.Select(l => (LightSource)new EdgeLight(l.Position, l.Direction, l.Width, l.Color)));

            LightSources = light_sources.AsReadOnly();
            TotalIllumination = LightSources.Sum(source => source.Illumination);
        }

        public Ray SampleLightRays(Random rand, ref bool from_directed)
        {
            float pick = (float)rand.NextDouble() * TotalIllumination;

            foreach(var light in LightSources)
            {
                if(pick < light.Illumination)
                {
                    var (pos, dir) = light.Sample(rand);

                    from_directed = light is DirectedLightSource;

                    return new Ray(pos + dir * 0.0001f, dir, light.Color, 0, 0);
                }

                pick -= light.Illumination;
            }

            var last_source = LightSources.Last();
            var (last_pos, last_dir) = last_source.Sample(rand);
            return new Ray(last_pos + last_dir * 0.0001f, last_dir, last_source.Color, 0, 0);
        }

        public Vec3 GetDirectedLight(Vec2 pos)
        {
            Vec3 result = new();

            foreach(var light in LightSources)
            {
                if(light is DirectedLightSource directed)
                {
                    if(!directed.Illuminates(pos))
                    {
                        continue;
                    }

                    float dist = (directed.Position - pos).Norm();

                    if (CheckHit(new Ray(pos, (directed.Position - pos).Normalized(), new(), 0, 0), dist))
                    {
                        result += directed.GetDirectLight(pos);
                    }
                }
            }

            return result;
        }

        public bool CheckHit(in Ray ray, float distance)
        {
            bool hit = false;
            var rec = GetWorldHit(ray, ref hit);

            return !hit || distance < rec.Distance;
        }

        public HitRecord GetWorldHit(in Ray ray, ref bool hit)
        {
            var poly_hit = false;
            var circle_hit = false;

            var poly_rec = GetPolygonHit(ray, ref poly_hit);
            var circle_rec = GetCircleHit(ray, ref circle_hit);

            float min_t = float.MaxValue;
            HitRecord rec = new();

            if (poly_hit && poly_rec.Distance < min_t)
            {
                min_t = poly_rec.Distance;
                hit = true;
                rec = poly_rec;
            }

            if (circle_hit && circle_rec.Distance < min_t)
            {
                min_t = circle_rec.Distance;
                hit = true;
                rec = circle_rec;
            }

            return rec;
        }

        private HitRecord GetCircleHit(in Ray ray, ref bool hit)
        {
            float min_t = float.MaxValue;
            hit = false;
            HitRecord rec = new();

            foreach (var circle in Circles)
            {
                HitRecord rec_temp = new();
                var hit_temp = circle.GetHit(ray, ref rec_temp);

                hit |= hit_temp;

                if (hit_temp && rec_temp.Distance < min_t)
                {
                    rec = rec_temp;
                    min_t = rec_temp.Distance;
                }
            }

            return rec;
        }

        private HitRecord GetPolygonHit(Ray ray, ref bool hit)
        {
            float min_t = float.MaxValue;
            hit = false;
            HitRecord rec = new();

            foreach (var polygon in Polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    HitRecord rec_temp = new();
                    var hit_temp = edge.GetHit(ray, polygon, ref rec_temp);

                    hit |= hit_temp;

                    if (hit_temp && rec_temp.Distance < min_t)
                    {
                        rec = rec_temp;
                        min_t = rec_temp.Distance;
                    }
                }
            }

            return rec;
        }
    }
}
