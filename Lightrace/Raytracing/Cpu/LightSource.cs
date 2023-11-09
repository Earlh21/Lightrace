using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    internal interface LightSource
    {
        public float Illumination { get; }
        public Vec3 Color { get; }
        public (Vec2, Vec2) Sample(Random rand);
    }
}
