using System;
using System.Collections.Generic;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("ttf|otf")]
    public class FontLoader : Ur.Filesystem.Loader {
        public FontLoader(string path)  : base(path) { }
        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var f = new Font(Path);            
            switch (f.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); break;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); break;
            }
        }
    }           
}
