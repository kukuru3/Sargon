using System;
using System.Collections.Generic;
using System.Linq;

namespace Sargon.Graphics {
    public class Canvas {
            
        #region Fields
        private List<IRenderable> activeItems;
        private IOrderedEnumerable<IRenderable> finalDrawList;
        private HashSet<IRenderable> flaggedForRemoval;
        private float zed;
        private bool  isRenderablesOrderDirty;
        private readonly Pipeline pipeline;
        #endregion
            
        #region Public properties
        /// <summary> Set to false if you want not to render this canvas. Note that canvases can still be rendered explicitly in a Protocol</summary>
        public bool DoesRender { get; set; } = true;
        /// <summary>"Zed" determins when canvas is rendered. Smaller zed is drawn earlier. Larger zed is drawn on top. </summary>
        public float Zed { get { return zed; } set { zed = value; pipeline.MarkCanvasListDirty(); } }

        public bool SortRenderables { get; set; } 
        #endregion

        #region Constructor
        internal Canvas(Pipeline p) {
            pipeline = p;
            activeItems = new List<IRenderable>();
            flaggedForRemoval = new HashSet<IRenderable>();
            zed = pipeline.GetAutomaticCanvasZed();
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

        #endregion

        #region Called by Sargon guts
        internal void MarkMemberDepthAsDirty() {
            if (!SortRenderables) return;
            isRenderablesOrderDirty = true;
        }

        internal virtual void Display() {
            BeginDisplay();
            DrawRenderables();
            FinalizeDisplay();
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
            finalDrawList = (IOrderedEnumerable<IRenderable>)activeItems;
            if (SortRenderables && isRenderablesOrderDirty) finalDrawList = finalDrawList.Where(item => item.Visible).OrderBy(r => r.Zed);
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
