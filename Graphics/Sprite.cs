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
        internal SFML.Graphics.Sprite nativeSprite;
        private Texture texture;        
        #endregion

        #region Properties
        public SpriteDefinition Source { get; set; }
        public Vector2 Scale { get; set; }
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
                         
        public void Display() {
            nativeSprite.Texture    = Source.Texture.NativeTexture;
            nativeSprite.Color      = Color.ToSFMLColor();
            nativeSprite.Rotation   = Rotation;
            nativeSprite.Position   = Position.ToSFMLVector2f();
            nativeSprite.Scale      = Scale.ToSFMLVector2f();    
            nativeSprite.TextureRect= (sourceImageSubrect ?? Source.Rect).ToSFMLIntRect() ;

            // DO RENDER!

        }

        public void OverrideTextureSubrect(Ur.Grid.Rect rect) {
            sourceImageSubrect = rect;
        }

        public void Dispose() {
            
        }
        
    }
}
