using System.Collections.Generic;
using System.Linq;

namespace Sargon.Graphics {


    public class Pipeline : State {

        /// <summary> The priority at which Pipeline processes the frame to render.
        /// Useful if you want to have stuff AFTER the frame is rendered. </summary>
        public const int PRIORITY = 9999;

        #region Fields
        private List<IPipelineStep> steps = null;
        private bool canvasListDirty = true;
        private float canvasAutoZed = 0f;
        #endregion

        #region Ctor
        internal Pipeline() {
            IsInternal = true;
            steps = new List<IPipelineStep>();
        }
        #endregion

        protected internal override void Initialize() {
            base.Initialize();
            Register(Hooks.Frame, ExecutePipeline, PRIORITY);
        }

        internal void MarkStepsDirty() => canvasListDirty = true;

        internal float GetAutomaticStepZed() => ++canvasAutoZed;

        private void ExecutePipeline() {

            if (canvasListDirty) {
                // do sorting here, etc.
                canvasListDirty = false;
                steps = steps.OrderBy(c => c.Zed).ToList();
            }
            foreach (var step in steps) if (step.DoesRender) ExecuteStep(step);
        }

        private void ExecuteStep(IPipelineStep step) {
            step.Display();
        }

        internal void AddStep(IPipelineStep step) {
            steps.Add(step);
            MarkStepsDirty();
        }

        public void RemoveStep(IPipelineStep step) {
            steps.Remove(step);
            MarkStepsDirty();
        }
    }
}
