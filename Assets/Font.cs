using System;
using System.Collections.Generic;

namespace Sargon.Assets {
    class Font : IAsset {   

        public Font(string filePath) {            
            Path = filePath;
            StartLoad();
        }

        
        private SFML.Graphics.Font sourceFont;

        private string Path { get; }

        public LoadStates LoadState { get; private set; }

        public void StartLoad() {
            
            if (sourceFont != null) Unload();

            LoadState = LoadStates.Loading;

            try {
                sourceFont = new SFML.Graphics.Font(Path);
            } catch (Exception) {
                LoadState = LoadStates.Failed;
            }
            
            LoadState = LoadStates.Active;
        }

        public void Unload() {
            LoadState = LoadStates.NotLoaded;
        }

        public void Dispose() {
            if (sourceFont != null) sourceFont.Dispose();
        }
    }
}
