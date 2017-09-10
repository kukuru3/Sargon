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
        private IntRect? sourceImageSubrect;
        private Ur.Grid.Coords? overriddenAnchor;
        internal SFML.Graphics.Sprite nativeSprite;
        #endregion

        #region Properties
        public SpriteDefinition Source { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Position { get; set; }
        public float   Rotation { get; set; }
        public Canvas  OnCanvas { get; set; }
        public Color   Color    { get; set; } = Color.White;
        public bool Visible { get; set; }
        public float Zed { get { return z; } set { if (z.Approximately(value)) return; z = value; OnCanvas?.MarkMemberDepthAsDirty(); } }        
        #endregion

        public Sprite() {
            nativeSprite = new SFML.Graphics.Sprite();
        }

        public void Fit(Rect toRect) {
            var r = sourceImageSubrect ?? Source.Rect;
            this.Scale = new Vector2(toRect.W / r.Width, toRect.H / r.Height);
            this.Position = new Vector2(toRect.X0, toRect.Y0);            
            
        }

        public void Display() {
            nativeSprite.Texture     = Source.Texture.NativeTexture;
            nativeSprite.Color       = Color.ToSFMLColor();
            nativeSprite.Rotation    = Rotation;
            nativeSprite.Position    = Position.ToSFMLVector2f();
            nativeSprite.Scale       = Scale.ToSFMLVector2f();
            nativeSprite.TextureRect = (sourceImageSubrect ?? Source.Rect).ToSFMLIntRect() ;
            var anchor = overriddenAnchor ?? Source.Anchor;
            nativeSprite.Origin      = new SFML.System.Vector2f(anchor.X, anchor.Y);
            // DO RENDER!

            OnCanvas?.Pipeline.Game.MainWindow.Draw(nativeSprite);
            
        }

        public void OverrideTextureSubrect(Ur.Grid.Rect rect) {
            sourceImageSubrect = rect;
        }

        public void OverrideAnchor(Ur.Grid.Coords anchor) {
            overriddenAnchor = anchor;
        }
        public void OverrideAnchor(int x, int y) {
            overriddenAnchor = new Ur.Grid.Coords(x, y);
        }

        public void ClearOverrides() {
            overriddenAnchor = null;
            sourceImageSubrect = null;
        }

        public void Dispose() {
            nativeSprite?.Dispose();
        }
    }
}
