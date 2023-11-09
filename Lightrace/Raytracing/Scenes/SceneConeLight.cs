using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Scenes
{
    public class SceneConeLight
    {
        public Vec2 Position { get; set; }
        public Vec2 Direction { get; set; }
        public float Angle { get; set; }
        public Vec3 Color { get; set; }

        public SceneConeLight(Vec2 position, Vec2 direction, float angle_degrees, Vec3 color)
        {
            Position = position;
            Direction = direction.Normalized();
            Angle = angle_degrees * (float)Math.PI / 180;
            Color = color;
        }

        public SceneConeLight(float x, float y, float dir_x, float dir_y, float angle_degrees, float r, float g, float b) : this(new Vec2(x, y), new Vec2(dir_x, dir_y), angle_degrees, new Vec3(r, g, b))
        {
            
        }

        public SceneConeLight(float x, float y, float dir_x, float dir_y, float angle_degrees, Vec3 color) : this(new Vec2(x, y), new Vec2(dir_x, dir_y), angle_degrees, color)
        {

        }
    }
}
