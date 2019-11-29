using System;
using System.Collections.Generic;
using SFK = SFML.Window.Keyboard.Key;
using Ur;
using System.Linq;

namespace Sargon.Input {
    internal static class Converter {

        internal static Keys[] ConstructSFMLKeyMap() {

            var maxValue = Enums.IterateValues<Keys>().Max();
            var table = new Keys[(int)maxValue + 1];

            for ( Keys i = 0; i <= maxValue; i++) table[(int)i] = Keys.NoKey;

            for (SFK key = SFK.A; key <= SFK.Z; key++) table[(int)key] = (Keys)((int)Keys.A + (int)(key - SFK.A));
            for (SFK key = SFK.Num0; key <= SFK.Num9; key++) table[(int)key] = (Keys)((int)Keys.Alpha0 + (int)(key - SFK.Num0));
            for (SFK key = SFK.F1; key <= SFK.F12; key++) table[(int)key] = (Keys)((int)Keys.F1 + (int)(key - SFK.F1));

            table[(int)SFK.Return] = Keys.Enter;
            table[(int)SFK.Space] = Keys.Space;
            table[(int)SFK.RAlt] = Keys.RAlt;
            table[(int)SFK.RControl] = Keys.RCtrl;
            table[(int)SFK.RShift] = Keys.RShift;
            table[(int)SFK.LAlt] = Keys.LAlt;
            table[(int)SFK.LControl] = Keys.LCtrl;
            table[(int)SFK.LShift] = Keys.LShift;
            table[(int)SFK.Tab] = Keys.Tab;
            table[(int)SFK.Left] = Keys.Left;
            table[(int)SFK.Right] = Keys.Right;
            table[(int)SFK.Up] = Keys.Up;
            table[(int)SFK.Down] = Keys.Down;
            table[(int)SFK.BackSpace] = Keys.Backspace;
            table[(int)SFK.Delete] = Keys.Delete;
            table[(int)SFK.Escape] = Keys.Escape;
            table[(int)SFK.Numpad0] = Keys.Pad0;
            table[(int)SFK.Numpad1] = Keys.Pad1;
            table[(int)SFK.Numpad2] = Keys.Pad2;
            table[(int)SFK.Numpad3] = Keys.Pad3;
            table[(int)SFK.Numpad4] = Keys.Pad4;
            table[(int)SFK.Numpad5] = Keys.Pad5;
            table[(int)SFK.Numpad6] = Keys.Pad6;
            table[(int)SFK.Numpad7] = Keys.Pad7;
            table[(int)SFK.Numpad8] = Keys.Pad8;
            table[(int)SFK.Numpad9] = Keys.Pad9;         
            table[(int)SFK.Add]     = Keys.PadPlus;
            table[(int)SFK.Subtract] = Keys.PadMinus;

            table[(int)SFK.LBracket] = Keys.LeftBracket;
            table[(int)SFK.RBracket] = Keys.RightBracket;
            table[(int)SFK.SemiColon] = Keys.Semicolon;
            table[(int)SFK.Quote] = Keys.Apostrophe;
            table[(int)SFK.Comma] = Keys.Comma;
            table[(int)SFK.Period] = Keys.Period;
            table[(int)SFK.Slash] = Keys.Slash;
            table[(int)SFK.BackSlash] = Keys.Backslash;
            table[(int)SFK.Equal] = Keys.Equals;
            // table[(int)SFK.Subtract] = Keys.Subtract;                                           
            
            return table;
        }


    }
}
