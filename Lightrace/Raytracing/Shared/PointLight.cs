using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Cpu;

namespace Lightrace.Raytracing.Shared
{
    internal struct PointLight : LightSource, DirectedLightSource
    {
        public Vec2 Position { get; }
        public Vec3 Color { get; }

        public float Illumination => 100;

        public PointLight(Vec2 position, Vec3 color)
        {
            Position = position;
            Color = color;
        }

        public (Vec2, Vec2) Sample(Random rand)
        {
            return (Position, Vec2.GetDir((float)Math.PI * 2 * (float)rand.NextDouble()));
        }

        public bool Illuminates(Vec2 point)
        {
            return true;
        }

        public Vec3 GetDirectLight(Vec2 point)
        {
            return Color * Illumination / (2 * (float)Math.PI * (Position - point).Norm());
        }
    }
}
