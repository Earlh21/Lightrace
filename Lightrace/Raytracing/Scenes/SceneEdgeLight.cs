using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Scenes
{
    public class SceneEdgeLight
    {
        public Vec2 Position { get; set; }
        public Vec2 Direction { get; set; }
        public float Width { get; set; }
        public Vec3 Color { get; set; }

        public SceneEdgeLight(Vec2 position, Vec2 direction, float width, Vec3 color)
        {
            Position = position;
            Direction = direction;
            Width = width;
            Color = color;
        }

        public SceneEdgeLight(float x, float y, float dir_x, float dir_y, float width, float r, float g, float b) : this(new Vec2(x, y), new Vec2(dir_x, dir_y), width, new Vec3(r, g, b))
        {

        }
        public SceneEdgeLight(float x, float y, float dir_x, float dir_y, float width, Vec3 color) : this(new Vec2(x, y), new Vec2(dir_x, dir_y), width, color)
        {

        }
    }
}
