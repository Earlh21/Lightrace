using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Cpu;

namespace Lightrace.Raytracing.Shared
{
    internal struct EdgeLight : LightSource, DirectedLightSource
    {
        public Vec2 Position { get; }
        public Vec2 Direction { get; }
        public float Width { get; }
        public Vec3 Color { get; }

        public float Illumination => 50;

        public EdgeLight(Vec2 position, Vec2 direction, float width, Vec3 color)
        { 
            Position = position;
            Direction = direction;
            Width = width;
            Color = color;
        }

        public (Vec2, Vec2) Sample(Random rand)
        {
            Vec2 pos = Position + (rand.NextSingle() - 0.5f) * Width * Direction.Rotate(MathF.PI / 2);
            return (pos, Direction);
        }

        public bool Illuminates(Vec2 point)
        {
            //Point is in the light's direction, and isn't outside the light's width
            return (point - Position) * Direction > 0 && MathF.Abs((point - Position).ScalarProjection(Direction)) / 2 < Width;
        }

        public Vec3 GetDirectLight(Vec2 point)
        {
            //The light doesn't spread over a larger distance, so illumination doesn't change based on position
            return Color * Illumination / Width;
        }
    }
}
