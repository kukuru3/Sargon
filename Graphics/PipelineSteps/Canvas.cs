using System;
using System.Collections.Generic;
using System.Linq;

namespace Sargon.Graphics {
    public class Canvas : BasicPipelineStep {
            
        #region Fields
        private List<IRenderable> activeItems;
        private List<IRenderable> finalDrawList;
        //private IOrderedEnumerable<IRenderable> finalDrawList;
        private HashSet<IRenderable> flaggedForRemoval;        
        private bool  isRenderablesOrderDirty;
        #endregion
            
        #region Public properties
        /// <summary> Set to false if you want not to render this canvas. Note that canvases can still be rendered explicitly in a Protocol</summary>
        
        public bool SortRenderables { get; set; } 
        #endregion

        #region Constructor
        public Canvas(Pipeline p) : base(p) {
            activeItems = new List<IRenderable>();
            flaggedForRemoval = new HashSet<IRenderable>();            
        }
        #endregion
            
        #region Public interface

        public void Add(IRenderable item) { 
            activeItems.Add(item); 
            item.OnCanvas = this;
            isRenderablesOrderDirty = true;
        }
             
        public void Remove(IRenderable item) { flaggedForRemoval.Add(item); item.OnCanvas = null; }
        
        public void Clear() { foreach (var item in activeItems) item.OnCanvas = null; activeItems.Clear(); flaggedForRemoval.Clear(); }

        public override void Display() {
            BeginDisplay();
            DrawRenderables();
            FinalizeDisplay();
        }

        #endregion

        #region Called by Sargon guts
        internal void MarkMemberDepthAsDirty() {
            if (!SortRenderables) return;
            isRenderablesOrderDirty = true;
        }

       
        #endregion

        #region Rendering
        protected virtual void FinalizeDisplay() {
            RemoveFlaggedRenderables();
        }

        protected virtual void DrawRenderables() {
            foreach (var renderable in finalDrawList) renderable.Display();
        }

        protected virtual void BeginDisplay() {
            finalDrawList = activeItems;
            if (SortRenderables && isRenderablesOrderDirty) finalDrawList = finalDrawList.Where(item => item.Visible).OrderBy(r => r.Zed).ToList();
            isRenderablesOrderDirty = false;
        }
        #endregion

        #region Guts
        private void RemoveFlaggedRenderables() {
            activeItems.RemoveAll(rr => flaggedForRemoval.Contains(rr));
            foreach (var item in flaggedForRemoval) item.OnCanvas = null;
            flaggedForRemoval.Clear();
        } 
        #endregion
            
    }
}
