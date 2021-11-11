using OpenGL;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace SharpEngine {
    public class Triangle {

        private Vertex[] vertices;

        public float CurrentScale { get; private set; }

        public Triangle(Vertex[] vertices) {
            this.vertices = vertices;
            CurrentScale = 1f;
            LoadTriangleIntoBuffer();
        }

        public Vector GetMinBounds() {
            var min = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) {
                min = Vector.Min(min, vertices[i].position);
            }
            return min;
        }

        public Vector GetMaxBounds() {
            var max = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) {
                max = Vector.Max(max, vertices[i].position);
            }
            return max;
        }

        public Vector GetCenter() {
            return (GetMinBounds() + GetMaxBounds()) / 2;
        }

        public void Scale(float multiplier) {
            var center = GetCenter();
            Move(center*-1);

            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].position *= multiplier;
            }

            Move(center);
            this.CurrentScale *= multiplier;
        }

        public void Move(Vector direction) {
            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].position += direction;
            }
        }

        public unsafe void Render() {
            fixed (Vertex* vertex = &vertices[0]) { //GL_STATIC_DRAW
                Gl.glBufferData(Gl.GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, Gl.GL_DYNAMIC_DRAW);
            }
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, vertices.Length);
        }

        private static unsafe void LoadTriangleIntoBuffer() {
            // load the vertices into buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), (void*)sizeof(Vector));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
        }
    }
}
