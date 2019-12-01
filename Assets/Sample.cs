using SA = SFML.Audio;

namespace Sargon.Assets {
    /// <summary>An audio sample.</summary>
    public class Sample : IAsset {

        SA.Sound sound;

        SA.SoundStream stream;

        public Sample(string path, bool streaming) {
            Path = path;
            if (streaming) {
                
                
            }
        }

        public string Path { get; }

        public LoadStates LoadState { get; private set; }

        public void Dispose() => throw new System.NotImplementedException();
        public void StartLoad() { }
        public void Unload() { }
    }
}
