using System;

namespace Sargon.Assets {
    public class Font : IAsset {

        public Font(string filePath) {
            Path = filePath;
            StartLoad();
        }

        internal SFML.Graphics.Font NativeFont { get; private set; }

        public string Path { get; }

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

        public float GetGlyphWidth(string glyph, int charSize) {
            var q = (uint)char.ConvertToUtf32(glyph, 0);
            var g = NativeFont.GetGlyph( q, (uint)charSize, false, 0f);
            return g.Advance;
        }

        public float GetLineHeight(int charSize) => NativeFont.GetLineSpacing((uint)charSize);

        public void Unload() {
            LoadState = LoadStates.NotLoaded;
        }

        public void Dispose() {
            if (NativeFont != null) NativeFont.Dispose();
        }

        public Metadata.FontMetadata Metadata { get; internal set; }

        Metadata.BaseMetadata IAsset.Metadata => Metadata;

        public static implicit operator Font(string assetID) => GameContext.Current.Assets.GetFont(assetID);
    }
}
