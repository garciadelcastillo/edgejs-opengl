using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;

namespace OpenGLOffScreenRendering
{
    internal static class Renderer
    {
        private static readonly float[] _vertices =
        {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };
        private static int _vertexBufferObject;

        private static int _vertexArrayObject;
        private static Shader? _shader;

        //private static readonly int Width = 600;
        //private static readonly int Height = 600;

        private static readonly bool OFFSCREEN = true;


        public static unsafe byte[] Triangle(int screenW, int screenH, Color clr, Color bgClr)
        {
            var nws = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(screenW, screenH),
                Title = "Offscreen Window",
                Flags = ContextFlags.ForwardCompatible, // This is needed to run on macos
                //Flags = ContextFlags.Offscreen,  // this doesn't have any effect??
                API = ContextAPI.OpenGL,
                APIVersion = Version.Parse("4.1")
            };

            Console.WriteLine(nws);
            Console.WriteLine(nws.API);
            Console.WriteLine(nws.APIVersion);

            GameWindow? window = null;
            GLFWGraphicsContext? context;
            if (OFFSCREEN)
            {
                // https://www.glfw.org/docs/3.3/context_guide.html

                GLFW.WindowHint(WindowHintBool.Visible, false);
                OpenTK.Windowing.GraphicsLibraryFramework.Window* winPtr = GLFW.CreateWindow(screenW, screenH, "Offscreen win", null, null);

                context = new GLFWGraphicsContext(winPtr);
                context?.MakeCurrent();
                GL.LoadBindings(new GLFWBindingsContext());

                Console.WriteLine(context);
            }
            else
            {
                var gws = GameWindowSettings.Default;
                Console.WriteLine(gws);

                window = new GameWindow(gws, nws);
                window.Context.MakeCurrent();
            }


            // Part of OnLoad()
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(0);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            _shader.Use();


            // Part of .OnResize()
            GL.Viewport(0, 0, screenW, screenH);

            // We skip OnUpdateFrame() as it only contains keyboard input handling

            // Part of .OnRenderFrame()
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Do this before swapping buffers: readpixels reads from the 
            // framebuffer, which is the back buffer. If you swap buffers
            // before reading, you'll read from the front buffer, which
            // is undefined.
            //float[] pixels = new float[Width * Height * 4];
            //GL.ReadPixels(0, 0, Width, Height, PixelFormat.Rgba, PixelType.Float, pixels);
            byte[] pixels = new byte[screenW * screenH * 4];
            GL.ReadPixels(0, 0, screenW, screenH, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            // Offscreen check
            int nonZeroCount = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] != 0) nonZeroCount++;
                //else
                //{
                //    Console.WriteLine(i - 3 + ": " + pixels[i - 3]);
                //    Console.WriteLine(i - 2 + ": " + pixels[i - 2]);
                //    Console.WriteLine(i - 1 + ": " + pixels[i - 1]);
                //    Console.WriteLine(i + ": " + pixels[i]);
                //}
            }
            Console.WriteLine("Count of non-zero pixels: " + nonZeroCount + " out of " + pixels.Length);

            if (!OFFSCREEN) window?.SwapBuffers();

            // Stop here
            Console.ReadKey();

            // Part of .OnUnload()
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);

            Console.WriteLine("End of triangle rendering...");

            return pixels;
        }
    }
}
