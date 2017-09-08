using System;
using System.Collections.Generic;
using System.Linq;

namespace Sargon.Graphics {
    public class Pipeline : State {

        /// <summary> The priority at which Pipeline processes the frame to render.
        /// Useful if you want to have stuff AFTER the frame is rendered. </summary>
        public const int PRIORITY = 99999;

        #region Fields
        private List<Canvas> canvasList = null;
        private bool canvasListDirty = true;
        private float canvasAutoZed = 0f;
        #endregion

        #region Ctor
        internal Pipeline() {
            IsInternal = true;
            canvasList = new List<Canvas>();
        } 
        #endregion

        protected internal override void Initialize() {
            base.Initialize();
            Register(Hooks.Frame, ExecutePipeline, PRIORITY);
        }

        internal void MarkCanvasListDirty() => canvasListDirty = true;

        internal float GetAutomaticCanvasZed() => ++canvasAutoZed;


        private void ExecutePipeline() {

            if (canvasListDirty) {
                // do sorting here, etc.
                canvasListDirty = false;                
                canvasList = canvasList.OrderBy(c => c.Zed).ToList();
            }
            foreach (var canvas in canvasList) if (canvas.DoesRender) RenderCanvas(canvas);         
        }

        private void RenderCanvas(Canvas canvas) {            
            canvas.Display();
        }
        
        public Canvas CreateCanvas() {
            var c = new Canvas(this);
            MarkCanvasListDirty();
            return c;
        }
    }
}
