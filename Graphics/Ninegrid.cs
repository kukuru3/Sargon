using System;
using Sargon.Assets;
using SFML.Graphics;
using Ur;
using Ur.Geometry;
using IntRect = Ur.Grid.Rect;

namespace Sargon.Graphics {
    public class Ninegrid : BaseSprite {

        SFML.Graphics.VertexArray nativeArray;

        Rect Rect { get; set; }

        private int textureMargin;
        private float renderedMargin;
        bool redraw = false;

        // the part of the texture that makes up edges and corners.
        public int TextureMargin { 
            get => textureMargin; 
            set {
                if (textureMargin == value) return;
                textureMargin = value;
                redraw = true;
            } 
        }

        // the corresponding part of the rendered rectangle
        public float RenderedMargin {
            get => renderedMargin;
            set {
                if (renderedMargin.Approximately(value)) return;
                renderedMargin = value;
                redraw = true;
            }
        }

        public Ninegrid() {
            nativeArray= new VertexArray(PrimitiveType.Quads);
        }

        // the full rectangle of the ninegrid, including margins
        public void SetRect(Rect rect) {
            Rect = rect;
            redraw = true;
        }

        public Vector2 Position => (Rect.X0, Rect.Y0);

        private void RebuildArray() {
            nativeArray.Clear();
            if (Source == null) return;

            var vertices = new Vertex[4,4];
            var texW = Source.Rect.Width;
            var texH = Source.Rect.Height;

            var sourceTexXValues = new[] { 0f, TextureMargin, texW - TextureMargin, texW };
            var sourceTexYValues = new[] { 0f, TextureMargin, texH - TextureMargin, texH };

            var xvalues = new[] { 0f, RenderedMargin, Rect.W - RenderedMargin, Rect.W };
            var yvalues = new[] { 0f, RenderedMargin, Rect.H - RenderedMargin, Rect.H };

            for (var i = 0; i < 4; i++) {
                for (var j = 0; j < 4; j++) {
                    vertices[i,j] = new Vertex(
                        new SFML.System.Vector2f(xvalues[i], yvalues[j]),
                        new SFML.System.Vector2f(sourceTexXValues[i], sourceTexYValues[j])
                    );
                }
            }

            void AddQuad(int indexX, int indexY) {
                nativeArray.Append(vertices[indexX, indexY]);
                nativeArray.Append(vertices[indexX+1, indexY]);
                nativeArray.Append(vertices[indexX+1, indexY+1]);
                nativeArray.Append(vertices[indexX, indexY+1]);
            }

            for (var i = 0; i < 3; i++) for (var j = 0; j < 3; j++) AddQuad(i, j);

        }

        public override void Display() {
            if (redraw) {
                RebuildArray();
                redraw = false;
            }

            var context = OnCanvas?.Pipeline.Context;
            if (context == null) return;
            context.Diagnostics.SpritesDrawn++;
            context.Renderer.RenderNinegrid(this, nativeArray);
        }

        public override void Dispose() {
            nativeArray.Dispose();
        }
    }
}
