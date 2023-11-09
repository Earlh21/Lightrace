using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Scenes
{
    public class ScenePointLight
    {
        public Vec2 Position { get; set; }
        public Vec3 Color { get; set; }

        public ScenePointLight(Vec2 position, Vec3 color)
        {
            Position = position;
            Color = color;
        }

        public ScenePointLight(float x, float y, Vec3 color) : this(new Vec2(x, y), color)
        {

        }

        public ScenePointLight(float x, float y, float r, float g, float b) : this(new Vec2(x, y), new Vec3(r, g, b))
        {

        }
    }
}
