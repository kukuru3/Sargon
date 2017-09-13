using System;
using System.Collections.Generic;

namespace Sargon.Assets {
    public class Font : IAsset {   

        public Font(string filePath) {            
            Path = filePath;
            StartLoad();
        }
                                                      
        internal SFML.Graphics.Font NativeFont { get; private set; }

        private string Path { get; }

        public LoadStates LoadState { get; private set; }

        public void StartLoad() {
            
            if (NativeFont != null) Unload();

            LoadState = LoadStates.Loading;

            try {
                NativeFont = new SFML.Graphics.Font(Path);
            } catch (Exception) {
                LoadState = LoadStates.Failed;
            }
            
            LoadState = LoadStates.Active;
        }

        public void Unload() {
            LoadState = LoadStates.NotLoaded;
        }

        public void Dispose() {
            if (NativeFont != null) NativeFont.Dispose();
        }
    }
}
