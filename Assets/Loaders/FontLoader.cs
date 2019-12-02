#define YAML
using Sargon.Assets.Metadata;
using System.IO;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("ttf|otf")]
    public class FontLoader : Ur.Filesystem.Loader<Font> {
        public FontLoader(string path) : base(path) { }
        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var f = new Font(Path);
            base.LoadedAssetItem = f;

            LoadedAssetMetadata = TryLoadMetadata();

            switch (f.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); break;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); break;
            }
        }

        private FontMetadata TryLoadMetadata() {
            var extraPath = this.Path + ".def";
            if (File.Exists(extraPath)) {
                var text = File.ReadAllText(extraPath);
                var input = new StringReader(text);
                #if YAML
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                var fontMetadata = deserializer.Deserialize<FontMetadata>(input);
                return fontMetadata;
                #endif
            } else {
                return null;
            }
        }
    }
}
