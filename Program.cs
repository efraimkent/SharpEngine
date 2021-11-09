using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine {
    class Program {

        static float[] vertices = new float[] {
            //vertex 1 x, y, z (left down)
            -.5f, -.5f, 0f,
            //vertex 2 x, y, z (
            .5f, -.5f, 0f,
            //vertex 3 x, y, z
            0f, .5f, 0f
        };

        static void Main(string[] args) {
            var window = CreateWindow();

            LoadTriangleIntoBuffer();

            LoadTriangleIntoBuffer();

            CreateShaderProgram();

            // engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                glClearColor(0, 0, 0, 1);
                glClear(GL_COLOR_BUFFER_BIT);
                glDrawArrays(GL_TRIANGLES, 0, 3);
                glFlush();
                //vertices[0] += 0.001f; //BL: x
                vertices[1] -= 0.001f; //BL: y
                //vertices[2] += 0.001f; //BL: z
                //vertices[3] += 0.001f; //BR: x
                vertices[4] -= 0.001f; //BR: y
                //vertices[5] += 0.001f; //BR: z
                //vertices[6] += 0.001f; //T: x
                vertices[7] -= 0.001f; //T: y
                //vertices[8] += 0.001f; //T: z

                UpdateTriangleBuffer();
            }
        }

        private static unsafe void LoadTriangleIntoBuffer() {

            // load the vertices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), NULL);

            glEnableVertexAttribArray(0);
        }

        static unsafe void UpdateTriangleBuffer() {
            fixed (float* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, vertex, GL_STATIC_DRAW);
            }
        }

        private static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/red-triangle.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/red-triangle.frag"));
            glCompileShader(fragmentShader);

            // create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        private static Window CreateWindow() {
            // initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.False);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}
