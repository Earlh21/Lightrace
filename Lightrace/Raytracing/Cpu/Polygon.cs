using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    public struct Polygon : LightSource
    {
        public IReadOnlyCollection<Edge> Edges { get; }
        public Material Material { get; }
        public int Id { get; }

        public float Perimeter { get; }
        public float Illumination => Material.Type == 1 ? Perimeter : 0;
        public Vec3 Color => Material.Color;

        public Polygon(IReadOnlyCollection<Edge> edges, Material material, int id)
        {
            Edges = edges;
            Material = material;
            Id = id;

            Perimeter = Edges.Sum(edge => edge.Length);
        }

        public (Vec2, Vec2) Sample(Random rand)
        {
            float pick = (float)rand.NextDouble() * Perimeter;

            foreach(var edge in Edges)
            {
                if (pick < edge.Length)
                {
                    Vec2 pos = edge.Start + (edge.End - edge.Start) * pick / edge.Length;
                    Vec2 dir = edge.Normal.Rotate(((float)rand.NextDouble() * 2 - 1) * (float)Math.PI);
                    return (pos, dir);
                }

                pick -= edge.Length;
            }

            return (new(), new());
        }

        public HitRecord GetHit(in Ray ray, ref bool hit)
        {
            float min_t = float.MaxValue;
            HitRecord rec = new();

            foreach (var edge in Edges)
            {
                HitRecord rec_temp = new();
                var hit_temp = edge.GetHit(ray, this, ref rec_temp);

                hit |= hit_temp;

                if (hit_temp && rec_temp.Distance < min_t)
                {
                    rec = rec_temp;
                    min_t = rec_temp.Distance;
                }
            }

            return rec;
        }
    }
}
