using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct HitRecord
    {
        public float Distance { get; }
        public Vec2 Position { get; }
        public Vec2 Normal { get; }
        public Vec2 RayDirection { get; }
        public Material Material { get; }
        public int IsInside { get; }
        public int Id { get; }

        public HitRecord(float distance, Vec2 position, Vec2 normal, Vec2 ray_direction, Material material, int is_inside, int id)
        {
            Distance = distance;
            Position = position;
            Normal = normal;
            RayDirection = ray_direction;
            Material = material;
            IsInside = is_inside;
            Id = id;
        }

        /// <summary>
        /// Transforms a ray off of this hit record.
        /// </summary>
        /// <param name="ray">Ray to transform</param>
        /// <param name="rand">Uniform random sampler</param>
        /// <returns>The new ray, and the end color of the old ray.</returns>
        public (Ray, Vec3) Transform(in Ray ray, Random rand)
        {
            float scatter_distance = Material.Scattering == 0 ? float.PositiveInfinity : -(float)Math.Log(rand.NextSingle()) / Material.Scattering;

            if (IsInside == 1 && scatter_distance < Distance)
            {
                var new_ray = Scatter(ray, scatter_distance, rand.NextSingle());
                return (new_ray, new_ray.Color);
            }

            if((float)rand.NextDouble() < Material.Transmission)
            {
                var new_ray = Refract(ray, rand.NextSingle());
                return (new_ray, new_ray.Color);
            }
            else
            {
                return (Bounce(ray, (float)rand.NextDouble()), ray.Color);
            }
        }

        public Ray Scatter(in Ray ray, float distance, float rand)
        {
            Vec2 pos = ray.Origin + ray.Direction * distance;
            Vec2 dir = Vec2.GetDir(rand * (float)Math.PI * 2);
            Vec3 color = ray.Color.Multiply(Material.Color.Pow(distance / 10));

            return new Ray(pos, dir, color, ray.Bounces, ray.Transmissions);
        }

        public Ray Bounce(in Ray ray, float rand)
        {
            rand = rand * 2 - 1;

            Vec2 rand_dir = Normal.Rotate(rand * (float)Math.PI / 2);
            Vec2 reflect_dir = ray.Direction - 2 * (ray.Direction * Normal) * Normal;

            Vec2 new_dir = (rand_dir * Material.Roughness + reflect_dir * (1 - Material.Roughness)).Normalized();

            return new Ray(Position - ray.Direction * 0.0001f, new_dir, ray.Color.Multiply(Material.Color), ray.Bounces + 1, ray.Transmissions);
        }

        public Ray Refract(in Ray ray, float rand)
        {
            rand = rand * 2 - 1;

            float n1 = IsInside == 1 ? Material.IOR : 1;
            float n2 = IsInside == 1 ? 1 : Material.IOR;

            Vec2 refract_dir = ray.Direction.Refract(Normal, n1, n2).Normalized();
            Vec2 rand_dir = (-Normal).Rotate(rand * (float)Math.PI / 2);

            Vec2 new_dir = (rand_dir * Material.Roughness + refract_dir * (1 - Material.Roughness)).Normalized();

            var color = ray.Color;

            if (IsInside == 1)
            {
                color = color.Multiply(Material.Color.Pow(Distance / 10));
            }

            return new Ray(Position + new_dir * 0.0001f, new_dir, color, ray.Bounces, ray.Transmissions + 1);
        }
    }
}
