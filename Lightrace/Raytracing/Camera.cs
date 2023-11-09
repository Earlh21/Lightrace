using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing
{
    public class Camera
    {
        public Vec2 Center { get; set; }
        public Vec2 Size { get; set; }

        public Camera(Vec2 center, Vec2 size)
        {
            Center = center;
            Size = size;
        }

        public Camera(float center_x, float center_y, float width, float height) : this(new Vec2(center_x, center_y), new Vec2(width, height))
        {

        }
    }
}
