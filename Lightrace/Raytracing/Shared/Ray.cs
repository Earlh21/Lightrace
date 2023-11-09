using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct Ray
    {
        public Vec2 Origin { get; }
        public Vec2 Direction { get; }
        public Vec3 Color { get; }
        public int Bounces { get; }
        public int Transmissions { get; }

        public Ray(Vec2 origin, Vec2 direction, Vec3 color, int bounces, int transmissions)
        {
            Origin = origin;
            Direction = direction.Normalized();
            Color = color;
            Bounces = bounces;
            Transmissions = transmissions;
        }
    }
}
