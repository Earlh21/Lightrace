using Lightrace.Raytracing;
using Lightrace.Raytracing.Cpu;
using Lightrace.Raytracing.Scenes;
using Lightrace.Raytracing.Shared;
using System.Drawing;
var settings = new RenderSettings(new Camera(0, 0, 10, 10), 200, 30000000, 6);
var renderer = new CpuRenderer(settings);

var scene = new Scene();

var glass = Material.CreateGlass(0, 1, 1.3f, 0, 1, 1, 1);
var mirror = Material.CreateWall(0, 1, 1, 1);
var red_dielectric = Material.CreateWall(0.2f, 1, 0, 0);
var blue_diffuse = Material.CreateWall(1, 0, 0, 1);
var gray_diffuse = Material.CreateWall(1, 0.8f, 0.8f, 0.8f);

scene.PointLights.Add(new(-4,3, 0.04f,0.04f,0.04f));
scene.Circles.Add(new(-2, 2, 0.8f, glass));
scene.Polygons.Add(ScenePolygon.CreateBox(2,0.2f,mirror).Rotated(MathF.PI * 0.55f).Translated(4,-1));
scene.Polygons.Add(ScenePolygon.CreateBox(10, 1, red_dielectric).Translated(0, -5));
scene.Polygons.Add(ScenePolygon.CreateBox(10, 1, blue_diffuse).Translated(0, 5));
scene.Polygons.Add(ScenePolygon.CreateBox(5,0.5f,gray_diffuse).Translated(-2.5f,0));


SaveImage("test", await renderer.RenderAsync(scene));

void SaveImage(string name, Vec3[,] colors)
{
    var image = new Bitmap(settings.Resolution.X, settings.Resolution.Y);
    for (int i = 0; i < settings.Resolution.X; i++)
    {
        for (int j = 0; j < settings.Resolution.Y; j++)
        {
            var pixel = colors[i, j];
            image.SetPixel(i, settings.Resolution.Y - j - 1, Color.FromArgb((byte)(pixel.X * 255), (byte)(pixel.Y * 255), (byte)(pixel.Z * 255)));
        }
    }

    image.Save($"{name}.png");
}

Environment.Exit(0);