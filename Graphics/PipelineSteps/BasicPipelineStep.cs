namespace Sargon.Graphics {

    public interface IPipelineStep {
        float Zed { get; }
        bool DoesRender { get; }
        void Display();
    }

    public abstract class BasicPipelineStep : IPipelineStep {
        private float zed = 0f;

        public BasicPipelineStep(Pipeline pipeline) {
            Pipeline = pipeline;
            Zed = Pipeline.GetAutomaticStepZed();
            Pipeline.AddStep(this);
        }

        public float Zed { get { return zed; } set { zed = value; Pipeline.MarkStepsDirty(); } }

        public bool DoesRender { get; set; } = true;

        protected internal Pipeline Pipeline { get; private set; }

        public abstract void Display();
    }
}
