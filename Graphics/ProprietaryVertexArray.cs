using SFML.Graphics;

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Sargon.Graphics {
    // If we were to stick to conventional .NET, every time we want to replace a vertex in a SFML.Net VertexArray
    // we'd have to use the indexer, which would call some extern. And since a Vertex is a struct we'd have to 
    // call it twice, once to get, once to set it back into the array.
    // In a tile-based roguelike (one use case for vertex array), we sometimes update hundreds of thousands of 
    // vertices per frame. The interop costs of calling externs repeatedly add up.

    // Therefore here we attempt to squeeze extra performance by returning a public (unsafe) pointer to the zeroth vertex
    // in the native vertex array C-side.

    class ProprietaryVertexArray : VertexArray {
        public ProprietaryVertexArray(PrimitiveType type) : base(type) { }

        // return pointer to the zeroth vertex in the source code
        public unsafe Vertex* ZeroVertexPointer() => sfVertexArray_getVertex(base.CPointer, 0);

        public unsafe Span<Vertex> Vertices => new Span<Vertex>(ZeroVertexPointer(), (int)VertexCount);

        // we copy over the extern import from VertexArray since it's sadly private.
        [DllImport("csfml-graphics", CallingConvention = CallingConvention.Cdecl)]
        [SuppressUnmanagedCodeSecurity]
        private unsafe static extern Vertex* sfVertexArray_getVertex(IntPtr CPointer, uint index);
    }
}
