using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lightrace.Raytracing.Shared;

namespace Lightrace.Raytracing.Cpu
{
    internal class GLDrawer
    {
        private RenderSettings settings;
        private Shader? shader = null;

        private CancellationTokenSource cts = new();

        private SemaphoreSlim thread_action_signal = new(0);
        private SemaphoreSlim copy_pixels_signal = new(0);

        private ConcurrentQueue<float[]> lines = new();

        private bool copy_pixels = false;
        private Vec3[,]? copied_pixels = null;

        public GLDrawer(RenderSettings settings)
        {
            this.settings = settings;
            cts = new();

            var thread = new Thread(() => GLLoop(cts.Token));
            thread.Start();
        }

        ~GLDrawer()
        {
            cts.Cancel();
        }

        public void QueueLines(float[] world_line_data)
        {
            lines.Enqueue(world_line_data);
            thread_action_signal.Release();
        }

        public async Task<Vec3[,]> GetPixels()
        {
            copy_pixels = true;
            thread_action_signal.Release();

            await copy_pixels_signal.WaitAsync();

            return copied_pixels;
        }

        private void GLLoop(CancellationToken ct)
        {
            var window = GLSetup();

            while(!ct.IsCancellationRequested)
            {
                thread_action_signal.Wait(ct);

                float[] world_line_data;
                while(lines.TryDequeue(out world_line_data))
                {
                    DrawLines(world_line_data);
                }

                if(copy_pixels)
                {
                    copy_pixels = false;
                    copied_pixels = CopyPixels();
                    copy_pixels_signal.Release();
                }
            }

            window.Dispose();
            GL.DeleteShader(shader?.Handle ?? 0);
        }

        private GameWindow GLSetup()
        {
            int w = settings.Resolution.X;
            int h = settings.Resolution.Y;

            var mode = new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 0, ColorFormat.Empty, 1);

            var window = new GameWindow(w, h, mode, "", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default)
            {
                Visible = false
            };

            window.MakeCurrent();

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.LineSmooth);

            int fb = GL.GenFramebuffer();

            int color_buffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, color_buffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, w, h, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, color_buffer, 0);

            GL.Viewport(0, 0, w, h);

            shader = new Shader("colored_line");

            return window;
        }

        private void DrawLines(float[] world_line_data)
        {
            //Transform to opengl coordinates
            for (int i = 0; i < world_line_data.Length; i += 6)
            {
                world_line_data[i] = (world_line_data[i] - settings.Camera.Center.X) / settings.Camera.Size.X * 2;
                world_line_data[i + 1] = (world_line_data[i + 1] - settings.Camera.Center.Y) / settings.Camera.Size.Y * 2;
            }

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, world_line_data.Length * sizeof(float), world_line_data, BufferUsageHint.StaticDraw);

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            shader.Use();

            GL.DrawArrays(PrimitiveType.Lines, 0, world_line_data.Length / 6);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
        }

        private Vec3[,] CopyPixels()
        {
            int w = settings.Resolution.X;
            int h = settings.Resolution.Y;

            var pixel_data = new float[w * h * 3];
            GL.ReadPixels(0, 0, w, h, PixelFormat.Rgb, PixelType.Float, pixel_data);

            var result = new Vec3[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int pos = (y * w + x) * 3;

                    result[x, y] = new Vec3(pixel_data[pos], pixel_data[pos + 1], pixel_data[pos + 2]);
                }
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            return result;
        }
    }
}
