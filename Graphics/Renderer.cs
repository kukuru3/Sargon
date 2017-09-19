using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Sargon.Graphics
{
    internal class Renderer {

        internal const bool UsePlaceholderSprite = true;

        public Pipeline Pipeline => GameContext.Current.Pipeline;

        public Renderer() {
            blitState = new SFML.Graphics.RenderStates();
            blitState.Transform = Transform.Identity;
            blitState.Shader = null;
            blitState.BlendMode = BlendMode.Add;

            if (UsePlaceholderSprite) placeholderSprite = new SFML.Graphics.Sprite();
               
        }

        SFML.Graphics.RenderStates blitState;

        SFML.Graphics.Sprite       placeholderSprite;


        internal void RenderSprite(Sprite sprite) {

            var s = UsePlaceholderSprite ? placeholderSprite : sprite.nativeSprite;

            s.Texture = sprite.Source.Texture.NativeTexture;
            s.Color = Color.White; // sprite.Color.ToSFMLColor();
            s.Rotation = sprite.Rotation;
            s.Position = sprite.Position.ToSFMLVector2f();
            s.Scale = sprite.Scale.ToSFMLVector2f();            
            s.TextureRect = sprite.TextureSubrect.ToSFMLIntRect();
            var anchor = sprite.Anchor;
            s.Origin = new SFML.System.Vector2f(anchor.X, anchor.Y);
            // DO RENDER!
            blitState.BlendMode =  sprite.Additive ? BlendMode.Add : BlendMode.Alpha;
            blitState.Texture = s.Texture;
            
            Pipeline.Game.RenderTarget.Draw(s, blitState);

        }

        internal void RenderText(Text text, SFML.Graphics.Text textSprite) {
            blitState.BlendMode = text.Additive ? BlendMode.Add : BlendMode.Alpha;
            Pipeline.Game.RenderTarget.Draw(textSprite);
        }
    }
}
