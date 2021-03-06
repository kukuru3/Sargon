﻿namespace Sargon.Input {
    public class Key {
        internal readonly Keys ID;
        public Status status;

        public Key(Keys code) {
            ID = code;
            status = Status.Idle;
        }

        public bool IsPressed => status == Status.Pressed;
        public bool IsHeld => status > Status.Idle && status <= Status.Held;
        public bool IsRaised => status >= Status.Raised;

        public enum Status {
            Idle,
            Pressed,
            Held,
            Raised,
        }

    }
}
