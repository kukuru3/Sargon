﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sargon.Graphics.PipelineSteps {
    public class RenderTextureStep : BasicPipelineStep {

        SFML.Graphics.RenderTexture renderTexture;

        // for now, rt will conform to the screen at 1 to 1 resolution
        public RenderTextureStep(Pipeline pipeline): base(pipeline) {
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

    public class DumpRTToScreenStep: BasicPipelineStep, IHasEffect {
        public DumpRTToScreenStep(Pipeline pipeline) : base(pipeline) { }

        public Effect Effect { get; set; }

        public override void Display() { 
            var mw = Pipeline.Context.GameInstance.MainWindow;
            if (Pipeline.RenderTargetStack.Peek() is SFML.Graphics.RenderTexture rt) {
                rt.Display();
                Pipeline.RenderTargetStack.Pop();
                Pipeline.Context.Renderer.RenderRect(rt, Ur.Geometry.Rect.FromDimensions(0, 0, mw.Size.X, mw.Size.Y), Effect);
            }
        }
    }
}
