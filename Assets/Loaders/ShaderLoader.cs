
namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("sfx")]
    public class ShaderLoader : Ur.Filesystem.Loader<Shader> {
        public ShaderLoader(string path): base(path) { }

        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var s = new Shader(Path);
            base.LoadedAssetItem = s;
            switch (s.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); break;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); break;
            }
        }

    }
}
