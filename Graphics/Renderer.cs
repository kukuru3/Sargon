﻿using System;
using SFML.Graphics;

namespace Sargon.Graphics {
    internal class Renderer {

        internal const bool UsePlaceholderSprite = true;

        public Pipeline Pipeline => GameContext.Current.Pipeline;

        public Renderer() {
            blitState = new SFML.Graphics.RenderStates();
            blitState.Transform = Transform.Identity;
            blitState.Shader = null;
            blitState.BlendMode = BlendMode.Add;

            if (UsePlaceholderSprite) placeholderSprite = new SFML.Graphics.Sprite();
            rectSprite = new SFML.Graphics.Sprite();
        }

        RenderStates blitState;

        SFML.Graphics.Sprite placeholderSprite;
        SFML.Graphics.Sprite rectSprite; // special sprite reused when ever we want to directly render a rect

        internal void RenderRect(RenderTexture t, Ur.Geometry.Rect rect, Effect shaderState, bool additive = false) {
            rectSprite.Texture = t.Texture;
            rectSprite.TextureRect = new IntRect(0, 0, (int)rect.W, (int)rect.H);
            rectSprite.Scale = new SFML.System.Vector2f(rect.W / t.Size.X, rect.H / t.Size.Y);
            rectSprite.Position = new SFML.System.Vector2f(rect.X0, rect.Y0);

            blitState.BlendMode = additive ? BlendMode.Add : BlendMode.Alpha;
            blitState.Shader = shaderState?.Shader?.NativeShader;
            shaderState?.Apply();
            Pipeline.RenderTarget.Draw(rectSprite, blitState);
        }

        internal void RenderRect(Assets.Texture source, Assets.Texture target, Ur.Geometry.Rect rect, Effect effect, bool additive = false) {
            if (target != null && !target.IsRenderTex) throw new Exception("Render target is invalid!");
            rectSprite.Texture = source.NativeTexture;
            rectSprite.TextureRect = new IntRect(0, 0, (int)rect.W, (int)rect.H);
            rectSprite.Scale = new SFML.System.Vector2f(rect.W / source.Width, rect.H / source.Height);
            rectSprite.Position = new SFML.System.Vector2f(rect.X0, rect.Y0);

            blitState.BlendMode = additive ? BlendMode.Add : BlendMode.Alpha;
            blitState.Shader = effect?.Shader?.NativeShader;
            effect?.Apply();
            if (target != null) target.NativeRenderTexture?.Draw(rectSprite, blitState);
            else Pipeline.RenderTarget.Draw(rectSprite, blitState);
        }

        internal void RenderQuadArray(QuadGrid qa) {
            var renderState = new RenderStates();
            renderState.BlendMode = BlendMode.Alpha;
            // renderState.BlendMode = qa.Additive ? BlendMode.Add : BlendMode.Alpha;
            // renderState.Shader = qa.Effect?.Shader?.NativeShader;
            // qa.Effect?.Apply();

            renderState.Transform = Transform.Identity;
            renderState.Transform.Translate(qa.Position.ToSFMLVector2f());
            renderState.Texture = qa.SourceGrid?.Texture?.NativeTexture;

            Pipeline.RenderTarget.Draw(qa.nativeVertexArray, renderState);
        }

        internal void RenderSprite(Sprite sprite) {

            var s = UsePlaceholderSprite ? placeholderSprite : sprite.nativeSprite;

            s.Texture = sprite.Source.Texture.NativeTexture;
            s.Color = sprite.Color.ToSFMLColor();
            s.Rotation = sprite.Rotation;
            s.Position = sprite.Position.ToSFMLVector2f();
            s.Scale = sprite.Scale.ToSFMLVector2f();
            s.TextureRect = sprite.TextureSubrect.ToSFMLIntRect();
            var anchor = sprite.Anchor;
            s.Origin = new SFML.System.Vector2f(s.TextureRect.Width * anchor.x, s.TextureRect.Height * anchor.y);

            // set up blit stat e
            blitState.BlendMode = sprite.Additive ? BlendMode.Add : BlendMode.Alpha;
            blitState.Shader = sprite.Effect?.Shader?.NativeShader;
            sprite.Effect?.Apply(); // applies itself to native shader instance.

            // DO RENDER!
            Pipeline.RenderTarget.Draw(s, blitState);
        }

        internal void RenderText(Text text, SFML.Graphics.Text textSprite) {
            blitState.BlendMode = text.Additive ? BlendMode.Add : BlendMode.Alpha;
            blitState.Shader = text.Effect?.Shader?.NativeShader;
            text.Effect?.Apply();

            Pipeline.RenderTarget.Draw(textSprite, blitState);
        }

        internal void RenderNinegrid(Ninegrid grid, VertexArray nativeArray) {
            var renderState = new RenderStates();
                
            renderState.BlendMode = grid.Additive ? BlendMode.Add : BlendMode.Alpha;
            renderState.Shader = grid.Effect?.Shader?.NativeShader;
            grid.Effect?.Apply();

            renderState.Transform = Transform.Identity;
            renderState.Transform.Translate(grid.Position.ToSFMLVector2f());
            renderState.Texture = grid.Source?.Texture?.NativeTexture;

            Pipeline.RenderTarget.Draw(nativeArray, renderState);
        }
    }
}
