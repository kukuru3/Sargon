using System;
using System.Collections.Generic;

namespace Sargon.Graphics.PipelineSteps {
    public class BackgroundColorSetter : BasicPipelineStep {
        Ur.Color Color { get; set; }
        public BackgroundColorSetter(Pipeline p, Ur.Color color) : base(p) {
            Color = color;
        }

        public override void Display() {
            Pipeline.Game.MainWindow.Clear(Color.ToSFMLColor());
        }
    }
}
