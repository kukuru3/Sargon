using System.Collections.Generic;
using System.Text.Unicode;

using Ur.Grid;

namespace Sargon.Assets {
    public class GridDefinition {
        private bool regen = true;
        private List<string> items = new List<string>();
        Dictionary<string, int> reverseLookup = new Dictionary<string, int>();

        public Texture Texture { get; }
        public string Identity { get; }
        /// <summary> Number of grid tiles.</summary>
        public Coords GridDimension { get; }

        public IEnumerable<string> Items => items;

        public GridDefinition(string identity, Texture texture, int dimensionX, int dimensionY) 
            : this(identity, texture, (dimensionX, dimensionY)) { }

        public GridDefinition(string identity, Texture texture, Coords dimensions) {
            Texture = texture;
            Identity = identity;
            GridDimension = dimensions;
            for (var i = 0; i < dimensions.X * dimensions.Y; i++) {
                items.Add(""); // preload with empty strings
            }
        }

        public void SetGlyphID(int row, int column, string glyphID) {
            items[row * GridDimension.X + column] = glyphID;
        }
        
        public int GetItemIndex(string item) {
            if (regen) RegenerateReverseLookup();
            if (!reverseLookup.TryGetValue(item, out var value)) return -1;
            return value;
        }

        private void RegenerateReverseLookup() {
            reverseLookup.Clear();
            for (var i = 0; i < items.Count; i++) reverseLookup[items[i]] = i;
        }

        public string GetItem(int index) {
            if (index < 0 || index > items.Count) return "";
            return items[index];
        }

        readonly SFML.System.Vector2f[] _uvs = new SFML.System.Vector2f[4]; // optimization

        internal void RecalculateUV(int index, ref SFML.System.Vector2f[] arr) {
            if (Texture == null) return;
            if (index == 0) return;

            var uvUnitX = 1f / GridDimension.X;
            var uvUnitY = 1f / GridDimension.Y;

            // unpack index to ix, iy:
            var iy = index / GridDimension.X;
            var ix = index - iy * GridDimension.X;

            var u0 = uvUnitX * ix;
            var v0 = uvUnitY * iy;

            u0 *= Texture.Width;
            v0 *= Texture.Height;
            uvUnitX *= Texture.Width;
            uvUnitY *= Texture.Height;

            arr[0] = new SFML.System.Vector2f(u0, v0);
            arr[1] = new SFML.System.Vector2f(u0 + uvUnitX, v0);
            arr[2] = new SFML.System.Vector2f(u0 + uvUnitX, v0 + uvUnitY);
            arr[3] = new SFML.System.Vector2f(u0, v0 + uvUnitY);
        }

        internal void RecalculateUV(string item, ref SFML.System.Vector2f[] arr) {
            var index = GetItemIndex(item);
            RecalculateUV(index, ref arr);
        }
    }
}
