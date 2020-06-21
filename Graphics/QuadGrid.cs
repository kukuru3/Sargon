using Sargon.Assets;

using SFML.Graphics;

using System.Collections.Generic;

using Ur;
using Ur.Geometry;
using Ur.Grid;

using SFVec2 = SFML.System.Vector2f;

namespace Sargon.Graphics {

    class QuadGridTileInfo {
        public uint color; 
        public int   glyphID;
        public bool  dirty;
        public int   packedArrayPosition;
    }

    public class QuadGrid : IRenderable {

        internal SFML.Graphics.VertexArray nativeVertexArray = new SFML.Graphics.VertexArray(SFML.Graphics.PrimitiveType.Quads);

        private float z;
        private bool visible = true;

        private Vector2 quadSize;
        private Coords numQuads;

        bool vertexUpdatePassRequired = true;
        bool allDirty = true;

        // there is a special grid definition
        public GridDefinition SourceGrid { get; set; }

        QuadGridTileInfo[,] tiles;

        public Vector2 Position { get; set; }

        public Ur.Color DefaultColor { get; set; }

        public Canvas OnCanvas { get; set; } = null;

        public Vector2 QuadDimensions { get => quadSize; set {quadSize = value; MarkAllDirty(); } }
        public Coords NumQuads { get => numQuads ; set { numQuads = value; MarkAllDirty(); RegenerateTiles(); } }

        public bool Visible {
            get => visible;
            set { visible = value; OnCanvas?.MarkMemberDepthAsDirty(); }
        }

        public float Zed { get { return z; } set { if (z.Approximately(value)) return; z = value; OnCanvas?.MarkMemberDepthAsDirty(); } }

        List<int> dirtyTiles = new List<int>();

        // There are two ways of updating the grid - either go through EVERY tile and checking whether the tile is dirty
        // OR going through a sparse hashset of updated tiles.
        // The phase where we go through a hashset is called "sparseApproach". 
        // Every dirtyness cycle starts sparse.
        // However, obviously if a large enough portion of the grid is dirty, it is far less efficient to go with maintaining a hashset.
        // In this case we rely on iterating through all tiles and checking for their 'dirty' property.
        // At the moment the threshold is a fixed number. 
        // For optimal performance, we should find a ratio at which maintaining a list of tiles
        bool sparseApproach = true;

        public QuadGrid() {
            DefaultColor = Ur.Color.Transparent;
        }

        void RegenerateTiles() {
            var xdim = quadSize.x; var ydim = quadSize.y;
            tiles = new QuadGridTileInfo[numQuads.X, numQuads.Y];
            nativeVertexArray.Clear();
            var c = DefaultColor.ToSFMLColor();
            dirtyTiles.Capacity = Numbers.Min(SPARSE_APPROACH_THRESHOLD, tiles.Length) + 2; 

            for (var iy = 0; iy < numQuads.Y; iy++)
            for (var ix = 0; ix < numQuads.X; ix++) {
                var x0 = xdim * ix; var x1 = xdim * (ix + 1);
                var y0 = ydim * iy; var y1 = ydim * (iy + 1);
                nativeVertexArray.Append(new SFML.Graphics.Vertex(Vec2(x0, y0), c));
                nativeVertexArray.Append(new SFML.Graphics.Vertex(Vec2(x1, y0), c));
                nativeVertexArray.Append(new SFML.Graphics.Vertex(Vec2(x1, y1), c));
                nativeVertexArray.Append(new SFML.Graphics.Vertex(Vec2(x0, y1), c));
                var t = new QuadGridTileInfo();
                tiles[ix, iy] = t;
                t.glyphID = 0;
                t.color  = DefaultColor;     
                t.packedArrayPosition  = iy * NumQuads.X + ix;   
            }
        }

        public void SetColor(int x, int y, uint c) {
            var t = tiles[x,y];
            t.color = c;
            if (!t.dirty) MarkDirty(t);
        }

        public void SetGlyph(int x, int y, string charID) {
            var index = SourceGrid.GetItemIndex(charID);
            
        }

        public void SetGlyph(int x, int y, int glyph) {
            if (glyph > -1) {
                var tile = tiles[x, y];
                tile.glyphID = glyph;
                if (!tile.dirty) MarkDirty(tile);
            }
        }

        const int SPARSE_APPROACH_THRESHOLD = 500;

        private void MarkAllDirty() {
            vertexUpdatePassRequired = true;
            allDirty = true;
            sparseApproach = false;
        }

        private void MarkDirty(QuadGridTileInfo tile) {
            tile.dirty = true;
            vertexUpdatePassRequired = true;
            if (sparseApproach) {
                dirtyTiles.Add(tile.packedArrayPosition);
                if (dirtyTiles.Count > SPARSE_APPROACH_THRESHOLD) sparseApproach = false;
            }
        }

        public void Display() { 
            if (vertexUpdatePassRequired) { 
                DoVertexUpdatePass();
                vertexUpdatePassRequired = false;
                dirtyTiles.Clear();
                allDirty = false;
                sparseApproach = true;
            }

            var context = OnCanvas?.Pipeline.Context;
            context?.Renderer.RenderQuadArray(this);
        }

        private void DoVertexUpdatePass() {
            bool hasTexture = SourceGrid != null;

            if (sparseApproach) {
                foreach (var tileIndex in dirtyTiles) { 
                    // unpack:
                    int y = tileIndex / numQuads.X; int x = tileIndex - y * numQuads.X;
                    RenderTileUpdates(hasTexture, this.tiles[x,y]);
                }
            }
            else {
                for (var ix = 0; ix < numQuads.X; ix++)
                for (var iy = 0; iy < numQuads.Y; iy++) {
                    var tile = tiles[ix, iy];
                    if (allDirty || tile.dirty)
                        RenderTileUpdates(hasTexture, tile);
                }
            }
        }

        // these are outside of functions for optimization purposes
        readonly Vertex[] vertices = new Vertex[4];
        SFVec2[] uvs;

        private void RenderTileUpdates(bool hasTexture, QuadGridTileInfo tile) {
            var index0 = (uint)tile.packedArrayPosition * 4u;

            for (var i = 0u; i < 4; i++)
                vertices[i] = nativeVertexArray[index0 + i];

            if (hasTexture) {
                uvs = SourceGrid.GetUV(tile.glyphID);
                for (var i = 0; i < 4; i++) vertices[i].TexCoords = uvs[i];
            }

            var color = ColorFromUint(tile.color);
            for (var i = 0u; i < 4; i++) {
                
                vertices[i].Color = color;
                // return index into the array
                nativeVertexArray[index0 + i] = vertices[i];
            }
            tile.dirty = false;
        }

        SFML.Graphics.Color ColorFromUint(uint color) => new SFML.Graphics.Color(color);
        SFVec2 Vec2(float x, float y) => new SFVec2(x, y);

        public void Dispose() { 
            nativeVertexArray.Dispose();
        }
    }
}
