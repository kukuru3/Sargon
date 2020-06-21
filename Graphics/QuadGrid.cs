using Sargon.Assets;

using SFML.Graphics;

using System;
using System.Collections.Generic;

using Ur;
using Ur.Geometry;
using Ur.Grid;

using SFVec2 = SFML.System.Vector2f;

namespace Sargon.Graphics {

    class QuadGridTileInfo {
        public uint  color; 
        public char  glyphID;
        public bool  dirty;
        public uint  packedArrayPosition;
    }

    public class QuadGrid : IRenderable {

        // internal VertexArray nativeVertexArray = new SFML.Graphics.VertexArray(SFML.Graphics.PrimitiveType.Quads);

        internal ProprietaryVertexArray nativeVertexArray = new ProprietaryVertexArray(PrimitiveType.Quads);

        private float z;
        private bool visible = true;

        private Vector2 quadSize;
        private Coords numQuads;

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

        List<uint> dirtyTiles = new List<uint>();

        public QuadGrid() {
            DefaultColor = Ur.Color.Transparent;

        }

        void RegenerateTiles() {
            var xdim = quadSize.x; var ydim = quadSize.y;
            tiles = new QuadGridTileInfo[numQuads.X, numQuads.Y];
            nativeVertexArray.Clear();
            var c = DefaultColor.ToSFMLColor();
            dirtyTiles.Capacity = tiles.Length + 2; 

            for (var iy = 0; iy < numQuads.Y; iy++)
            for (var ix = 0; ix < numQuads.X; ix++) {
                var x0 = xdim * ix; var x1 = xdim * (ix + 1);
                var y0 = ydim * iy; var y1 = ydim * (iy + 1);
                nativeVertexArray.Append(new Vertex(Vec2(x0, y0), c));
                nativeVertexArray.Append(new Vertex(Vec2(x1, y0), c));
                nativeVertexArray.Append(new Vertex(Vec2(x1, y1), c));
                nativeVertexArray.Append(new Vertex(Vec2(x0, y1), c));
                var t = new QuadGridTileInfo();
                t.glyphID = (char)0;
                t.color  = DefaultColor;
                t.packedArrayPosition  = (uint)(iy * NumQuads.X + ix);
                tiles[ix, iy] = t;
            }
        }

        public void SetColor(int x, int y, uint c) {
            var t = tiles[x,y];
            t.color = c;
            if (!t.dirty) {
                t.dirty = true;
                dirtyTiles.Add(t.packedArrayPosition);
            }
        }

        public void SetGlyph(int x, int y, string charID) {
            var index = SourceGrid.GetItemIndex(charID);
            SetGlyph(x, y, (char)index);
        }

        public void SetGlyph(int x, int y, char glyph) {
            if (glyph > -1) {
                var t = tiles[x, y];
                t.glyphID = glyph;
                if (!t.dirty) {
                    dirtyTiles.Add(t.packedArrayPosition);
                    t.dirty = true;
                }
            }
        }

        bool allDirty = false;

        private void MarkAllDirty() => allDirty = true;

        public void Display() { 
            DoVertexUpdatePass();
            var context = OnCanvas?.Pipeline.Context;
            context?.Renderer.RenderQuadArray(this);
        }

        const bool USE_UNSAFE_UPDATE = true;

        unsafe Vertex* firstVertexPointer;
       
        private unsafe void DoVertexUpdatePass() {
            firstVertexPointer = nativeVertexArray.ZeroVertexPointer();
            
            bool hasTexture = SourceGrid != null;
            if (allDirty) {
                for (var x = 0; x < NumQuads.X; x++)
                    for (var y = 0; y < NumQuads.Y; y++) { 
                        var t = tiles[x,y];
                        RenderTileUpdatesUnsafe(hasTexture, t);
                        t.dirty = false;
                    }
            } else {
                for (int i = 0, n = dirtyTiles.Count; i < n; i++) {
                    var tileIndex = dirtyTiles[i];
                    int y = (int)tileIndex / numQuads.X; int x = (int)tileIndex - y * numQuads.X;
                    var t = tiles[x,y];
                    RenderTileUpdatesUnsafe(hasTexture, t);
                    t.dirty = false;
                }
            }
            
            dirtyTiles.Clear();
            allDirty = false;
        }

        // these are outside of functions for optimization purposes
        readonly Vertex[] vertices = new Vertex[4];
        SFVec2[] uvs = new SFVec2[4];
        SFML.Graphics.Color _recyclablecolor;
        private unsafe void RenderTileUpdatesUnsafe(bool hasTexture, QuadGridTileInfo tile) {
            uint index = tile.packedArrayPosition * 4;

            if (hasTexture) SourceGrid.RecalculateUV(tile.glyphID, ref uvs);
            
            _recyclablecolor = ColorFromUint(tile.color);
            for (var i = 0u; i < 4; i++) {
                firstVertexPointer[index+i].TexCoords = uvs[i];
                firstVertexPointer[index+i].Color = _recyclablecolor;
            }
        }

        SFML.Graphics.Color ColorFromUint(uint color) => new SFML.Graphics.Color(color);
        SFVec2 Vec2(float x, float y) => new SFVec2(x, y);

        public void Dispose() { 
            nativeVertexArray.Dispose();
        }
    }
}
