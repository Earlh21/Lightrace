using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing
{
    public class RenderSettings
    {
        public int SampleCount { get; set; }
        public int MaxBounces { get; set; }
        public float ImageScale { get; set; }
        public Camera Camera { get; set; }

        public Vec2i Resolution => (Vec2i)(Camera.Size * ImageScale);

        public RenderSettings(Camera camera, float image_scale = 1, int sample_count = 200, int max_bounces = 4)
        {
            ImageScale = image_scale;
            Camera = camera;
            SampleCount = sample_count;
            MaxBounces = max_bounces;
        }
    }
}
