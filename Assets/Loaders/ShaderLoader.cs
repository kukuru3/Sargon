#define YAML
using Sargon.Assets.Metadata;
using System.IO;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("sfx")]
    public class ShaderLoader : Ur.Filesystem.Loader<Shader> {
        public ShaderLoader(string path): base(path) { }

        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var s = new Shader(Path);
            base.LoadedAssetItem = s;
            base.LoadedAssetMetadata = TryLoadMetadata();
            switch (s.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); break;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); break;
            }
        }

        private ShaderMetadata TryLoadMetadata() {
            var extraPath = this.Path + ".def";
            if (File.Exists(extraPath)) {
                var text = File.ReadAllText(extraPath);
                var input = new StringReader(text);
                #if YAML
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                var shaderMetadata = deserializer.Deserialize<ShaderMetadata>(input);
                return shaderMetadata;
                #endif
            } else {
                return null;
            }
        }

    }
}
