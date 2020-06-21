using Sargon.Assets.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Ur.Filesystem;
using Ur.Grid;

namespace Sargon.Assets {
    public class AssetDatabase {

        #region Lookup lists and dictionaries
        private List<SpriteDefinition> allSpriteDefinitions;
        private Dictionary<Texture, List<SpriteDefinition>> spriteDefsByTexture;
        private Dictionary<string, SpriteDefinition> spriteDefsByStringID;
        private Dictionary<string, GridDefinition> gridDefsByStringID;
        private Dictionary<Texture, string> textureIdentitiesLookup;
        private Dictionary<string, Font> fontsCachedByID;
        private Dictionary<string, Sample> samplesCachedByID;
        private Dictionary<string, Shader> shadersCachedByID;

        private List<IAsset> allAssets;
        #endregion

        public int DefaultCharacterSize { get; set; } = 22;
        public Font DefaultFont { get; set; }

        public IEnumerable<Font> AllFonts => fontsCachedByID.Values.OrderBy(font => font.Path);

        #region Ctor
        public AssetDatabase() {
            allSpriteDefinitions = new List<SpriteDefinition>();
            spriteDefsByTexture = new Dictionary<Texture, List<SpriteDefinition>>();
            spriteDefsByStringID = new Dictionary<string, SpriteDefinition>();
            gridDefsByStringID = new Dictionary<string, GridDefinition>();
            fontsCachedByID = new Dictionary<string, Font>();
            samplesCachedByID = new Dictionary<string, Sample>();
            shadersCachedByID = new Dictionary<string, Shader>();
            textureIdentitiesLookup = new Dictionary<Texture, string>();
            allAssets = new List<IAsset>();
        }
        #endregion

        #region Handlers
        internal void HandleAssetLoaded(Loader loader, object asset, object metadata) {
            if (asset is IAsset a ) {
                TryAssignMetadata(a, metadata);
                allAssets.Add(a);
            }

            switch (asset) {
                case Texture t: HandleTextureLoaded(loader, t); break;
                case Font f: HandleFontLoaded(loader, f); break;
                case Shader s: HandleShaderLoaded(loader, s); break;
                case Sample ss: HandleSampleLoaded(loader, ss); break;
            }
        }

        private void TryAssignMetadata(IAsset a, object metadata) {
            switch (a) {
                case Texture t: t.Metadata = metadata as TextureMetadata; break;
                case Font f: f.Metadata = metadata as FontMetadata; break;
                case Shader s: s.Metadata = metadata as ShaderMetadata; break;
                case Sample ss: ss.Metadata = metadata as SampleMetadata; break;
            }
        }

        private void HandleSampleLoaded(Loader loader, Sample asset) {
            var idkey = GetAssetIdentityKey(loader);
            samplesCachedByID.Add(idkey, asset);
        }

        private void HandleFontLoaded(Loader loader, Font asset) {
            if (DefaultFont == null) DefaultFont = asset;

            var idkey = GetAssetIdentityKey(loader);
            fontsCachedByID.Add(idkey, asset);
        }

        private void HandleShaderLoaded(Loader loader, Shader asset) {
            var idkey = GetAssetIdentityKey(loader);
            shadersCachedByID.Add(idkey, asset);
        }

        private void HandleTextureLoaded(Loader loader, Texture asset) {
            lock(spriteDefsByTexture) { 
                spriteDefsByTexture[asset] = new List<SpriteDefinition>();
                var idkey = GetAssetIdentityKey(loader);
                textureIdentitiesLookup[asset] = idkey;
                spriteDefsByStringID.Add(idkey, asset);
            }
        }

        #endregion
        /// <summary> For dynamically created textures</summary>
        /// <param name="asset"></param>
        public Texture CreateDynamicTexture(int width, int height, string identity) {
            var t = new Texture(width, height, Ur.Color.White);
            textureIdentitiesLookup[t] = identity;
            spriteDefsByStringID.Add(identity, t);
            return t;
        }

        #region Sprite definition stuff
        public void AddSpriteDefinition(Texture texture, Ur.Grid.Rect subrect, string identity) {
            var l = spriteDefsByTexture[texture];
            var def = new SpriteDefinition(identity, texture, subrect, l.Count);
            l.Add(def);
            allSpriteDefinitions.Add(def);
            spriteDefsByStringID.Add(textureIdentitiesLookup[texture] + ":" + identity, def);
        }

        public SpriteDefinition Find(Texture texture, string identity) {
            if (identity == "") return texture;
            // might need a lookup later on:
            return spriteDefsByTexture[texture].FirstOrDefault(sd => sd.Identity == identity);
        }

        public SpriteDefinition Find(Texture texture, int sequence) {
            if (sequence == -1) return texture;
            var l = spriteDefsByTexture[texture];
            return l[sequence % l.Count];
        }
        public SpriteDefinition Find(string totalID) {
            spriteDefsByStringID.TryGetValue(totalID, out var value);
            return value;
        }
        #endregion

        public void AddGridDefinition(Texture texture, string identity, Coords dimensions) {
            var griddef = new GridDefinition(identity, texture, dimensions);
            gridDefsByStringID.Add(identity, griddef);
        }

        public GridDefinition GetGrid(string identity) {
            gridDefsByStringID.TryGetValue(identity, out var value);
            return value;
        }

        public Font GetFont(string assetIdentity) {
            fontsCachedByID.TryGetValue(assetIdentity, out var value);
            return value;
        }

        public Shader GetShader(string assetIdentity) {
            shadersCachedByID.TryGetValue(assetIdentity, out var value);
            return value;
        }

        public Sample GetSample(string assetIdentity) {
            samplesCachedByID.TryGetValue(assetIdentity, out var value);
            return value;
        }

        private string GetAssetIdentityKey(Ur.Filesystem.Loader loader) {
            var raw = loader.Path.Substring(loader.BasePath.Length + 1);
            raw = raw.Replace('\\', '.').Replace('/', '.');
            var extPos = raw.LastIndexOf('.');
            raw = raw.Substring(0, extPos);
            return raw;
        }

        public IEnumerable<IAsset> WithTag(string tag) {
            return allAssets.Where(ass => ass.Metadata?.tags.Contains(tag) ?? false);
        }
    }
}
