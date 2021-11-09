using System;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine {
    class Program {
        static void Main(string[] args) {
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);

            var window = Glfw.CreateWindow(1024, 756, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);

            float[] vertices = new float[] {
                -.5f, -.5f, 0f,
                .5f, -.5f, 0f,
                0f, .5f, 0f
            };

            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            unsafe {
                fixed (float * vertex = &vertices[0]) {
                    glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, vertex, GL_STATIC_DRAW);
                }
                glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);
            }
            glEnableVertexAttribArray(0);

            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to changes (position etc)
                glDrawArrays(GL_TRIANGLES, 0, 3);
                Glfw.SwapBuffers(window);
            }
        }
    }
}
