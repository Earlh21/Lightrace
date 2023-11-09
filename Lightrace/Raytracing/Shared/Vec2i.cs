using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Shared
{
    public struct Vec2i
    {
        public int X { get; }
        public int Y { get; }

        public Vec2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2i operator -(Vec2i a, Vec2i b) => new Vec2i(a.X - b.X, a.Y - b.Y);
    }
}
