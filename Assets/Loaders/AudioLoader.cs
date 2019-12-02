using System;
using System.IO;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("wav|mpg|ogg|flac")]
    class AudioLoader : Ur.Filesystem.Loader<Sample> {

        const long streamingCutoffPoint = 10 * 1024 * 1024;

        public AudioLoader(string path) : base(path) { }

        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);

            var meta = TryLoadMetadata(Path);
            bool isLargeFile = new FileInfo(Path).Length > streamingCutoffPoint;

            var isStreamingAsset = meta?.streamed ?? isLargeFile;

            var sample = new Sample(Path, isStreamingAsset);
            base.LoadedAssetItem = sample;

            UpdateState(Ur.Filesystem.LoadStates.Completed);
        }

        private SampleMetadata TryLoadMetadata(string path) {
            var extraPath = this.Path + ".def";
            if (File.Exists(extraPath)) {
                var text = File.ReadAllText(extraPath);
                var input = new StringReader(text);
                
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                var sampleMetadata = deserializer.Deserialize<SampleMetadata>(input);
                return sampleMetadata;
            }
            return null;
        }

        class SampleMetadata {
            public bool streamed { get; set; }
        }
    }
}
