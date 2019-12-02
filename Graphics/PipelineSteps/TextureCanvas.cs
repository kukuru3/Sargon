using Ur.Geometry;

namespace Sargon.Graphics {
    public class TextureCanvas : Canvas {

        Vector2 origin;
        Ur.Grid.Coords size;
        bool doRegenerate;

        public Effect Effect { get; set; } = null;

        public void Move(Vector2 offset) => origin += offset;
        public void Resize(int x, int y) { 
            var newSize = (x, y);
            if (newSize != size) { 
                size = newSize;
                doRegenerate = true; 
            }
        }

        SFML.Graphics.RenderTexture target;

        public Rect Rect { 
            get => Rect.FromDimensions(origin.x, origin.y, size.X, size.Y);
            set {
                origin = (value.X0, value.Y0);
                size = new Ur.Grid.Coords((int)value.W, (int)value.H);
                doRegenerate = true;
            }
        }

        private void RegenerateTexture() {
            if (target != null) { 
                target.Texture.Dispose();
                target.Dispose();
                target = null;
            }
            var w = (uint)size.X;
            var h = (uint)size.Y;
            if (w < 1) w = 1;
            if (h < 1) h = 1;
            target = new SFML.Graphics.RenderTexture(w, h);
        }

        public TextureCanvas(Rect rect) {
            this.Rect = rect; // will also execute a texture regeneration
        }

        protected override void BeginDisplay() {
            if (doRegenerate) {
                RegenerateTexture();
                doRegenerate = false;
            }
            target.Clear(SFML.Graphics.Color.Transparent);
            Pipeline.RenderTargetStack.Push(target);
            
            base.BeginDisplay();
        }

        protected override void FinalizeDisplay() {
            base.FinalizeDisplay();
            target.Display();
            Pipeline.RenderTargetStack.Pop();

            Pipeline.Context.Renderer.RenderRect(target, Rect, Effect);
        }
    }
}
