namespace Sargon.Graphics.PipelineSteps {
    public class BackgroundColorSetter : BasicPipelineStep {
        Ur.Color Color { get; set; }
        public BackgroundColorSetter(Ur.Color color) {
            Color = color;
        }

        public override void Display() {
            Pipeline.RenderTarget.Clear(Color.ToSFMLColor());
        }
    }
}
