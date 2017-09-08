using System;
using System.Collections.Generic;
using Ur;
using Ur.Grid;    

namespace Sargon.Input {
    public class Manager : State {

        Keys[] sfmlKeyMap;
        Key[]  keyStates;
        private HashSet<Key> activeKeys;

        public Coords MousePosition { get; private set; }
        public Coords MouseDelta { get; private set;    }
        public float  MouseWheel { get; private set;    }


        protected internal override void Initialize() {
            activeKeys = new HashSet<Key>();
            IsInternal = true;
            CreateMaps();
            Register(Hooks.Frame, ProcessInput , -1000);
            Register(Hooks.Frame, ResetInput, 99999);            
        }

        private void CreateMaps() {
            sfmlKeyMap = Converter.ConstructSFMLKeyMap();
            var maxKey = Enums.MaxValue<Keys>();
            keyStates = new Key[maxKey+1];
            for (var i = 0; i <= maxKey;i++) {
                keyStates[i] = new Key((Keys)i);
            }
        }
        
        private void ResetInput() {
            
            foreach (var key in activeKeys) {
                if      (key.status == Key.Status.Pressed) key.status = Key.Status.Held;                
                else if (key.status == Key.Status.Raised) key.status = Key.Status.Idle;                
            }

            MouseWheel = 0f;            
        }

        public Key GetKey(Keys keys) {
            return keyStates[(int)keys];
        }

        private void ProcessInput() {
            CancelKeys();
        }

        internal void AcceptNewMousePosition(int x, int y) {
            var pos = new Coords(x, y);
            MouseDelta =  pos - MousePosition;
            MousePosition = pos;
        }

        internal void AcceptWheelDelta(float delta) {
            MouseWheel += delta;
        }

        internal void CancelKeys() {
            activeKeys.RemoveWhere(k => k.status == Key.Status.Idle);
        }

        internal Keys Decode(SFML.Window.Keyboard.Key sfmlKey) {
            var index = (int)sfmlKey;
            if (index >= 0 && index < sfmlKeyMap.Length) return sfmlKeyMap[index];
            return Keys.NoKey;
        }

    }
}
