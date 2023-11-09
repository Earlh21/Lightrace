using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Scenes
{
    public class SceneCircle
    {
        public Vec2 Position { get; }
        public float Radius { get; }
        public Material Material { get; }

        public SceneCircle(Vec2 position, float radius, Material material)
        {
            Position = position;
            Radius = radius;
            Material = material;
        }

        public SceneCircle(float x, float y, float radius, Material material) : this(new Vec2(x, y), radius, material)
        {

        }
    }
}
