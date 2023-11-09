using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Cpu;

namespace Lightrace.Raytracing.Shared
{
    public struct Circle : LightSource
    {
        public Vec2 Position { get; }
        public float Radius { get; }
        public Material Material { get; }
        public int Id { get; }

        public float Perimeter => (float)Math.PI * 2 * Radius;
        public float Illumination => Material.Type == 1 ? Perimeter : 0;
        public Vec3 Color => Material.Color;

        public Circle(Vec2 position, float radius, Material material, int id)
        {
            Position = position;
            Radius = radius;
            Material = material;
            Id = id;
        }

        public Circle(float x, float y, float radius, Material material, int id) : this(new Vec2(x, y), radius, material, id)
        {

        }

        public (Vec2, Vec2) Sample(Random rand)
        {
            Vec2 dir = Vec2.GetDir((float)Math.PI * 2 * (float)rand.NextDouble());
            Vec2 pos = Position + dir * Radius;
            return (pos, dir.Rotate(((float)rand.NextDouble() - 0.5f) * (float)Math.PI));
        }

        public float GetAngleBreadth(Vec2 position)
        {
            float distance = (Position - position).Norm();

            if (distance < Radius)
            {
                return (float)Math.PI * 2;
            }

            return (float)Math.Atan(Radius / distance) * 2;
        }

        public bool GetHit(in Ray ray, ref HitRecord record)
        {
            var a = ray.Direction.NormSquared();
            var b = 2 * ray.Direction.X * (ray.Origin.X - Position.X) + 2 * ray.Direction.Y * (ray.Origin.Y - Position.Y);
            var c = (ray.Origin - Position).NormSquared() - Radius * Radius;

            var disc = b * b - 4 * a * c;

            if (disc <= 0)
            {
                return false;
            }

            var t = 2 * c;

            if (c >= 0)
            {
                t /= -b + (float)Math.Sqrt(disc);
            }
            else
            {
                t /= -b - (float)Math.Sqrt(disc);
            }

            if (t > 0)
            {
                var hit_pos = ray.Origin + ray.Direction * t;
                var normal = (hit_pos - Position).Normalized();

                if (c < 0)
                {
                    normal *= -1;
                }

                record = new HitRecord(t, hit_pos, normal, ray.Direction, Material, c < 0 ? 1 : 0, Id);
                return true;
            }

            return false;
        }
    }
}
