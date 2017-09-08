using System;
using System.Collections.Generic;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("png|bmp|jpg")]
    public class TextureLoader : Ur.Filesystem.Loader<Texture> {
        public TextureLoader(string path) : base(path) { }
        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var texture = new Texture(Path);
            base.LoadedAssetItem = texture;
            switch(texture.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); return;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); return;
            }
        }
        
    }
}
