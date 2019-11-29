using System.Collections.Generic;
using System.Linq;

namespace Sargon.Assets {
    public class AssetManager {

        #region Lookup lists and dictionaries
        private List<SpriteDefinition> allSpriteDefinitions;
        private Dictionary<Texture, List<SpriteDefinition>> spriteDefsByTexture;
        private Dictionary<string, SpriteDefinition> spriteDefsByStringID;
        private Dictionary<Texture, string> textureIdentitiesLookup;
        private Dictionary<string, Font> fontsCachedByID;
        #endregion

        public int DefaultCharacterSize { get; set; } = 22;
        public Font DefaultFont { get; set; }

        public IEnumerable<Font> AllFonts => fontsCachedByID.Values.OrderBy(font => font.Path);

        #region Ctor
        public AssetManager() {
            allSpriteDefinitions = new List<SpriteDefinition>();
            spriteDefsByTexture = new Dictionary<Texture, List<SpriteDefinition>>();
            spriteDefsByStringID = new Dictionary<string, SpriteDefinition>();
            fontsCachedByID = new Dictionary<string, Font>();
            textureIdentitiesLookup = new Dictionary<Texture, string>();
        }
        #endregion

        #region Handlers
        internal void HandleAssetLoaded(Ur.Filesystem.Loader loader, object asset) {
            if (asset is Texture) {
                HandleTextureLoaded(loader, (Texture)asset);
            } else if (asset is Font) {
                HandleFontLoaded(loader, (Font)asset);
            }

        }

        private void HandleFontLoaded(Ur.Filesystem.Loader loader, Font asset) {
            if (DefaultFont == null) DefaultFont = asset;

            var idkey = GetAssetIdentityKey(loader);
            fontsCachedByID.Add(idkey, asset);
        }

        private void HandleTextureLoaded(Ur.Filesystem.Loader loader, Texture asset) {
            spriteDefsByTexture[asset] = new List<SpriteDefinition>();
            var idkey = GetAssetIdentityKey(loader);
            textureIdentitiesLookup[asset] = idkey;
            spriteDefsByStringID.Add(idkey, asset);
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

        public Font GetFont(string assetIdentity) {
            fontsCachedByID.TryGetValue(assetIdentity, out var value);
            return value;
        }

        private string GetAssetIdentityKey(Ur.Filesystem.Loader loader) {
            var raw = loader.Path.Substring(loader.BasePath.Length + 1);
            raw = raw.Replace('\\', '.').Replace('/', '.');
            var extPos = raw.LastIndexOf('.');
            raw = raw.Substring(0, extPos);
            return raw;
        }

    }
}
