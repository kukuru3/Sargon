﻿#define YAML

using Sargon.Assets.Metadata;
using System.Collections.Generic;
using System.IO;
using Ur.Grid;

namespace Sargon.Assets.Loaders {
    [Ur.Filesystem.UrLoader("png|bmp|jpg")]
    public class TextureLoader : Ur.Filesystem.Loader<Texture> {
        public TextureLoader(string path) : base(path) { }
        protected override void Load() {
            UpdateState(Ur.Filesystem.LoadStates.Started);
            var texture = new Texture(Path);
            base.LoadedAssetItem = texture;
            switch (texture.LoadState) {
                case LoadStates.Active: UpdateState(Ur.Filesystem.LoadStates.Completed); break;
                case LoadStates.Failed: UpdateState(Ur.Filesystem.LoadStates.Failure); break;
            }
            var texMetadata = TryLoadAutomaticMetadata(texture);
            if (texMetadata != null) {
                base.LoadedAssetMetadata = texMetadata;
                ProcessTextureMetadata(texture, texMetadata);
            }
        }

        private void ProcessTextureMetadata(Texture texture, TextureMetadata texMetadata) {
            if (texMetadata.sprites != null) {
                foreach (var kvp in texMetadata.sprites) {
                    var itemKey = kvp.Key;
                    var itemRect = new Rect(kvp.Value[0], kvp.Value[1], kvp.Value[2], kvp.Value[3]);
                    GameContext.Current.Assets.AddSpriteDefinition(texture, itemRect, itemKey);
                }
            }
            if (texMetadata.grid != null) {
                var w = texMetadata.grid.chars_w;
                var h = texMetadata.grid.chars_h;
                var w1 = texture.Width / w;
                var h1 = texture.Height / h;

                for (var i = 0; i < h; i++)
                    for (var j = 0; j < w; j++) {
                        var str = texMetadata.grid.rows[i][j];
                        if (!string.IsNullOrEmpty(str)) {
                            var itemKey = str;
                            var itemRect = new Rect(j * w1, i * h1, w1, h1);
                            GameContext.Current.Assets.AddSpriteDefinition(texture, itemRect, itemKey);
                        }
                    }
            }
        }

        private TextureMetadata TryLoadAutomaticMetadata(Texture texture) {
            var extraPath = this.Path + ".def";
            if (File.Exists(extraPath)) {
                var text = File.ReadAllText(extraPath);
                var input = new StringReader(text);
                #if YAML
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                var texMetadata = deserializer.Deserialize<TextureMetadata>(input);
                return texMetadata;
                #endif
            } else {
                return null;
            }
        }
    }
}