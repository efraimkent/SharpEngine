using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine {
    class Program {

        private static Vector[] vertices = new Vector[] {

            new Vector(-.1f, -.1f),
            new Vector(.1f, -.1f),
            new Vector(0f, .1f),
            // new Vector(.4f, .4f),
            // new Vector(.6f, .4f),
            // new Vector(.5f, .6f)
        };

        static void Main() {
            var window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();

            // engine rendering loop
            var direction = new Vector(0.01f, 0.01f);
            var multiplier = 0.999f;
            var scale = 1f;
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);

                // 1. Scale the Triangle without Moving it

                // 1.1 Move the Triangle to the Center, so we can scale it without Side Effects
                // 1.1.1 Find the Center of the Triangle
                // 1.1.1.1 Find the Minimum and Maximum
                var min = vertices[0];
                for (var i = 1; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i]);
                }
                var max = vertices[0];
                for (var i = 1; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i]);
                }
                // 1.1.1.2 Average out the Minimum and Maximum to get the Center
                var center = (min + max) / 2;
                // 1.1.2 Move the Triangle the Center
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i] -= center;
                }
                // 1.2 Scale the Triangle
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i] *= multiplier;
                }
                // 1.3 Move the Triangle Back to where it was before
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i] += center;
                }

                // 2. Keep track of the Scale, so we can reverse it
                scale *= multiplier;
                if (scale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (scale >= 1f) {
                    multiplier = 0.999f;
                }

                // 3. Move the Triangle by its Direction
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i] += direction;
                }

                // checks X-Bounds of window
                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].x >= 1 && direction.x > 0 || vertices[i].x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                        break;
                    }
                }
                // checks Y-Bounds of window
                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].y >= 1 && direction.y > 0 || vertices[i].y <= -1 && direction.y < 0) {
                        direction.y *= -1;
                        break;
                    }
                }

                UpdateTriangleBuffer();
                EscapeWindow(window);
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
            glShaderSource(fragmentShader, File.ReadAllText("shaders/purple.frag"));
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
