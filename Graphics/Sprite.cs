using System;
using System.Collections.Generic;
using Sargon.Assets;
using Ur.Geometry;
using Ur;
using IntRect = Ur.Grid.Rect;

namespace Sargon.Graphics {
    public class Sprite : IRenderable {

        #region Fields
        private float z;
        private bool  visible = true;
        private IntRect? sourceImageSubrect;
        private Vector2? overriddenAnchor;
        internal SFML.Graphics.Sprite nativeSprite;
        #endregion

        #region Properties
        public SpriteDefinition Source { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Position { get; set; }
        public float   Rotation { get; set; } = 0;
        public Canvas  OnCanvas { get; set; } = null;
        public Color   Color    { get; set; } = Color.White;

        public bool Visible     {
            get => visible;
            set { visible = value; OnCanvas?.MarkMemberDepthAsDirty(); }
        }

        public float Zed { get { return z; } set { if (z.Approximately(value)) return; z = value; OnCanvas?.MarkMemberDepthAsDirty(); } }

        public IntRect TextureSubrect => sourceImageSubrect ?? Source.Rect;
        public Vector2 Anchor => overriddenAnchor ?? Source.Anchor;

        public bool Additive { get; set; }
        #endregion

        public Sprite() {
            if (!Renderer.UsePlaceholderSprite) {
                this.nativeSprite = new SFML.Graphics.Sprite();
            }
        }

        public void Fit(Rect toRect) {
            if (Source == null) return;
            var r = sourceImageSubrect ?? Source.Rect;
            this.Scale = new Vector2(toRect.W / r.Width, toRect.H / r.Height);
            this.Position = new Vector2(toRect.X0 + toRect.W * Anchor.x, toRect.Y0 + toRect.H * Anchor.y);
        }

        public void Fit(float w, float h) {
            var r = sourceImageSubrect ?? Source.Rect;
            this.Scale = new Vector2(w / r.Width, h / r.Height);
        }
        
        public void Display() {
            var context = OnCanvas?.Pipeline.Context;
            if (context == null) return;

            context.Diagnostics.SpritesDrawn++;
            context.Renderer.RenderSprite(this);
        }

        public void OverrideTextureSubrect(Ur.Grid.Rect rect) {
            sourceImageSubrect = rect;
        }

        public void OverrideAnchor(Vector2 anchor) {
            overriddenAnchor = anchor;
        }
        public void OverrideAnchor(float x, float y) {
            overriddenAnchor = new Vector2(x, y);
        }

        public void ClearOverrides() {
            overriddenAnchor = null;
            sourceImageSubrect = null;
        }

        public void Dispose() {
            if (Renderer.UsePlaceholderSprite) return;
            nativeSprite?.Dispose();
        }
    }
}
