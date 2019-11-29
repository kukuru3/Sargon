using System;
using System.Collections.Generic;

namespace Sargon.Input {

    public enum Keys {

        NoKey,

        Left, Right, Up, Down,

        Escape, Space, Enter, Tab,
        LCtrl, LShift, LAlt, LSystem,
        RCtrl, RShift, RAlt, RSystem,
        
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,
        Pad0, Pad1, Pad2, Pad3, Pad4, Pad5, Pad6, Pad7, Pad8, Pad9,
        PadPlus, PadMinus, PadEnter,
        
        Backspace, Delete, Insert,

        LeftBracket, RightBracket, Semicolon, Apostrophe, Comma, Period, Slash, Backslash, Equals, Subtract,
                                                                                                            
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, Mouse7, Mouse8,
        Joy1, Joy2, Joy3, Joy4, Joy5, Joy6, Joy7, Joy8,                
    }

    public enum Axes {
        None,
        MouseX, 
        MouseY,
        MouseWheel,
        Joy1, 
        Joy2, 
        Joy3, 
        Joy4    
    }

}
