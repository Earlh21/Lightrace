using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Cpu;

namespace Lightrace.Raytracing.Shared
{
    internal struct ConeLight : LightSource, DirectedLightSource
    {
        public Vec2 Position { get; }
        public Vec2 Direction { get; }
        public float Angle { get; }
        public Vec3 Color { get; }

        public float Illumination => 50;

        public ConeLight(Vec2 position, Vec2 direction, float angle, Vec3 color)
        {
            Position = position;
            Direction = direction;
            Angle = angle;
            Color = color;
        }

        public (Vec2, Vec2) Sample(Random rand)
        {
            return (Position, Direction.Rotate(((float)rand.NextDouble() * 2 - 1) * Angle));
        }

        public bool Illuminates(Vec2 point)
        {
            return (point - Position).AngleBetween(Direction) < Angle;
        }

        public Vec3 GetDirectLight(Vec2 point)
        {
            return Color * Illumination / (Angle * (point - Position).Norm());
        }
    }
}
