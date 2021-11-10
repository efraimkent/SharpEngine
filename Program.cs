using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine {
    class Program {
        struct Vector {
            public float x, y, z;

            public Vector(float x, float y, float z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Vector(float x, float y) {
                this.x = x;
                this.y = y;
                this.z = 0;
            }
        }

        private static Vector[] vertices = new Vector[] {

            new Vector(-.1f, -.1f),
            new Vector(.1f, -.1f),
            new Vector(0f, .1f),
            new Vector(.4f, .4f),
            new Vector(.6f, .4f),
            new Vector(.5f, .6f)
        };

        private const int vertexSize = 3;
        private const int vertexX = 0;
        private const int vertexY = 1;
        private const int vertexZ = 2;

        static void Main() {
            var window = CreateWindow();

            LoadTriangleIntoBuffer();

            LoadTriangleIntoBuffer();

            CreateShaderProgram();

            // engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);

                //TriangleMoveXpos();

                TriangleMoveYneg();

                //TriangleScaleXYneg();

                //TriangleScaleXYpos();

                UpdateTriangleBuffer();
                EscapeWindow(window);
            }
        }

        private static void TriangleScaleXYpos() {
            // scale triangle up
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].x += 1.01f;
            }
        }

        private static void TriangleScaleXYneg() {
            // scale triangle down
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].x *= 0.99f;
            }
        }

        private static void TriangleMoveYneg() {
            // move triangle down
            for (var i = vertexY; i < vertices.Length; i++) {
                vertices[i].y -= 0.001f;
            }
        }

        private static void TriangleMoveXpos() {
            // move triangle right
            for (var i = vertexX; i < vertices.Length; i++) {
                vertices[i].x += 0.001f;
            }
        }

        private static void Render(Window window) {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
            //glFlush();
        }

        private static void ClearScreen() {
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static float[] RotateVector2D(float x, float y, float degrees) {
            float[] result = new float[2];
            result[0] = (float) (x * Math.Cos(degrees * Math.PI / 180f) - y * Math.Sin(degrees * Math.PI / 180f));
            result[1] = (float) (x * Math.Sin(degrees * Math.PI / 180f) + y * Math.Cos(degrees * Math.PI / 180f));
            return result;
        }

        private static void EscapeWindow(Window window) {
            //press escape to close window
            if (Glfw.GetKey(window, Keys.Escape) == InputState.Press)
                Glfw.SetWindowShouldClose(window, true);
        }

        private static unsafe void LoadTriangleIntoBuffer() {
            // load the vertices into buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vector), NULL);

            glEnableVertexAttribArray(0);
        }

        static unsafe void UpdateTriangleBuffer() {
            fixed (Vector* vertex = &vertices[0]) {                                                    //GL_STATIC_DRAW
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
        }

        private static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/blue.frag"));
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
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}
