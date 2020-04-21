using Sargon.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sargon.Graphics.PipelineSteps {
    public class RenderTextureStep : BasicPipelineStep {

        SFML.Graphics.RenderTexture renderTexture;

        // for now, rt will conform to the screen at 1 to 1 resolution
        public RenderTextureStep() {
            RegenerateRT();
        }

        private void RegenerateRT() {
            if (renderTexture != null) renderTexture.Dispose();
            var desiredW = (uint)Pipeline.Context.Screen.Width;
            var desiredH = (uint)Pipeline.Context.Screen.Height;
            renderTexture = new SFML.Graphics.RenderTexture(desiredW, desiredH);
        }

        public override void Display() {
            renderTexture.Clear(Ur.Color.Transparent.ToSFMLColor());
            Pipeline.RenderTargetStack.Push(renderTexture);
        }
    }

    public class DumpRTStep: BasicPipelineStep, IHasEffect {
        public Effect Effect { get; set; }
        public bool Additive { get; set; }
        public bool CollapseStack { get; set; }
        public override void Display() { 
            var mw = Pipeline.Context.GameInstance.MainWindow;
            if (Pipeline.RenderTargetStack.Peek() is SFML.Graphics.RenderTexture rt) {
                rt.Display();
                Pipeline.RenderTargetStack.Pop();
                if (CollapseStack) Pipeline.RenderTargetStack.Clear();
                Pipeline.Context.Renderer.RenderRect(rt, Ur.Geometry.Rect.FromDimensions(0, 0, mw.Size.X, mw.Size.Y), Effect, Additive);
            }
        }
    }

    public class BlitStep : BasicPipelineStep, IHasEffect {
        public Assets.Texture Source { get; set; }
        public Assets.Texture Target { get; set; }
        public Effect Effect { get; set; }
        public bool Additive { get; set; }

        public override void Display() {
            if (Source.IsRenderTex) Source.ApplyRenderTargetChanges();
            Pipeline.Context.Renderer.RenderRect(Source, Target, (0, 0, Source.Width, Source.Height), Effect, Additive);
        }
    }

    public class SetRenderTarget : BasicPipelineStep {
        public Texture Target { get; set; }

        public override void Display() {
            if (Target.IsRenderTex) Pipeline.RenderTargetStack.Push(Target.NativeRenderTexture);
        }
    }

    public class ClearTexture : BasicPipelineStep {
        public Ur.Color Color { get; set; }
        public Texture Target { get; set; }
        public override void Display() { 
            if (Target.IsRenderTex) Target.Clear(Color);
        }
    }

    public class ClearRenderTargetStack : BasicPipelineStep {
        public override void Display() { Pipeline.RenderTargetStack.Clear(); } 
    }

}
