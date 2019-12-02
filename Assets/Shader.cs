using System;

namespace Sargon.Assets {
    /// <summary>A thin wrapper around SFML.Graphics.Shader.</summary>
    public class Shader : IAsset {

        internal SFML.Graphics.Shader NativeShader { get; private set; }

        public string Path { get; }

        public LoadStates LoadState { get; internal set; }

        public Shader(string path) {
            Path = path;
            StartLoad();
        }

        public void Dispose() => NativeShader?.Dispose();

        public void StartLoad() {
            if (NativeShader != null) Unload();
            LoadState = LoadStates.Loading;
            try {
                NativeShader = new SFML.Graphics.Shader(null, null, Path);
            } catch (Exception) {
                LoadState = LoadStates.Failed;
            }

            LoadState = LoadStates.Active;

        }

        public void Unload() { 
            Dispose();
            NativeShader = null;
            LoadState = LoadStates.NotLoaded;
        }
    }
}
