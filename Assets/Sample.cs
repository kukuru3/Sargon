using SA = SFML.Audio;

namespace Sargon.Assets {
    /// <summary>An audio sample.</summary>
    public class Sample : IAsset {

        internal SA.SoundBuffer SoundBuffer { get; private set; }
        
        public bool Streaming { get; }

        public Sample(string path, bool streaming) {
            Path = path;
            Streaming = streaming;
            StartLoad();
        }

        public string Path { get; }

        public LoadStates LoadState { get; private set; }

        public void Dispose() {
            SoundBuffer?.Dispose();
        }

        public void StartLoad() { 
            if (!Streaming) {
                SoundBuffer = new SA.SoundBuffer(Path);
            }
        }

        public void Unload() { 
            LoadState = LoadStates.NotLoaded;
            Dispose();
        }
    }
}
