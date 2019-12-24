using System;
using System.Collections.Generic;
using System.Text;

namespace Sargon.Graphics {
    public enum Cursors {
        None,
        Normal,
        Hand,
        IBeam,
    }

    static internal class CursorUtilities {
        static internal SFML.Window.Cursor.CursorType ConvertCursor(Cursors c) {
            switch (c) {
                case Cursors.Hand: return SFML.Window.Cursor.CursorType.Hand;
                case Cursors.IBeam: return SFML.Window.Cursor.CursorType.Text;
                default: return SFML.Window.Cursor.CursorType.Arrow;
            }
        }
    }
}
