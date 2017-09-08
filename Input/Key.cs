using System;
using System.Collections.Generic;

namespace Sargon.Input {
    public class Key {
        internal readonly Keys ID;
        internal Status   status;

        public Key(Keys code) {
            ID = code;
            status = Status.Idle;
        }
        
        public bool IsPressed => status ==  Status.Pressed;
        public bool IsHeld    => status > 0 && status <= Status.Held;
        public bool IsRaised  => status >= Status.Raised;

        public enum Status {
            Idle,
            Pressed,            
            Held,
            Raised,            
        }

    }
}
