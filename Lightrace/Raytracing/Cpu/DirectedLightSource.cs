using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    internal interface DirectedLightSource
    {
        public Vec2 Position { get; }
        public bool Illuminates(Vec2 point);
        public Vec3 GetDirectLight(Vec2 point);
    }
}
