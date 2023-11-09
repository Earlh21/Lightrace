using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightrace.Raytracing.Scenes
{
    public class Scene
    {
        public List<ScenePolygon> Polygons { get; set; } = new();
        public List<SceneCircle> Circles { get; set; } = new();
        public List<ScenePointLight> PointLights { get; set; } = new();
        public List<SceneConeLight> ConeLights { get; set; } = new();
        public List<SceneEdgeLight> EdgeLights { get; set; } = new();
    }
}
