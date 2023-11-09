using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Scenes;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing
{
    public abstract class Renderer
    {
        public RenderSettings RenderSettings { get; set; }

        public Renderer(RenderSettings settings)
        {
            RenderSettings = settings;
        }

        public abstract Task<Vec3[,]> RenderAsync(Scene scene);
    }
}
