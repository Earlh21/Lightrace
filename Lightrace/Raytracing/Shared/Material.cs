using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct Material
    {
        //Light: 1
        //Wall: 2
        public int Type { get; }
        public float Roughness { get; }
        public float Transmission { get; }
        public float IOR { get; }

        public float Scattering { get; }
        public Vec3 Color { get; }

        public Material(int type, float roughness, float transmission, float ior, float scattering, Vec3 color)
        {
            Type = type;
            Roughness = roughness;
            Transmission = transmission;
            IOR = ior;
            Scattering = scattering;
            Color = color;
        }

        public Material(int type, float roughness, float transmission, float ior, float scattering, float r, float g, float b) : this(type, roughness, transmission, ior, scattering, new Vec3(r, g, b))
        { }

        public static Material CreateWall(float roughness, float r, float g, float b)
        {
            return new Material(2, roughness, 0, 0, 0, r, g, b);
        }

        public static Material CreateLight(Vec3 color)
        {
            return new Material(1, 0, 0, 0, 0, color.X, color.Y, color.Z);
        }

        public static Material CreateLight(float r, float g, float b)
        {
            return CreateLight(new Vec3(r, g, b));
        }

        public static Material CreateGlass(float roughness, float transmission, float ior, float scattering, float r, float g, float b)
        {
            return new Material(2, roughness, transmission, ior, scattering, r, g, b);
        }

        public Vec2 Sample(Vec2 ray_dir, Vec2 normal, float choice_rand, float sample_rand)
        {
            return normal.Rotate(SampleDiffuse(sample_rand));
        }

        public float Pdf(Vec2 ray_dir, Vec2 normal, Vec2 out_dir)
        {
            return PdfDiffuse();
        }

        public float SampleDiffuse(float rand)
        {
            float angle = rand * (float)Math.PI - (float)Math.PI / 2;

            return angle;
        }

        public float PdfDiffuse()
        {
            return 1.0f;
        }

        private float SampleVisibleNormal(float s, float xi)
        {
            float a = (float)Math.Tanh(-(float)Math.PI / 2 / (2 * s));
            float b = (float)Math.Tanh((float)Math.PI / 2 / (2 * s));

            return 2 * s * (float)Math.Atanh(a + (b - a) * xi);
        }

        public Vec2 SampleRoughMirror(float w_o, float rand)
        {
            float angle = SampleVisibleNormal(Roughness, rand);
            float pdf = 1;

            return new Vec2(angle, pdf);
        }
    }
}
