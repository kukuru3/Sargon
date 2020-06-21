using System.Collections.Generic;
using Ur;
using Ur.Grid;

namespace Sargon.Input {
    public class Manager : State {

        Keys[] sfmlKeyMap;
        Key[] keyStates;
        private HashSet<Key> activeKeys;

        public Coords MousePosition { get; private set; }
        public Coords MouseDelta { get; private set; }
        public float MouseWheel { get; private set; }


        protected internal override void Initialize() {
            activeKeys = new HashSet<Key>();
            IsInternal = true;
            CreateMaps();
            Register(Hooks.Frame, PrepareInput, -99999);
            Register(Hooks.Frame, FinalizeInput, 99999);
        }

        private void CreateMaps() {
            sfmlKeyMap = Converter.ConstructSFMLKeyMap();
            var maxKey = Enums.MaxValue<Keys>();
            keyStates = new Key[maxKey + 1];
            for (var i = 0; i <= maxKey; i++) {
                keyStates[i] = new Key((Keys)i);
            }
        }

        private void FinalizeInput() {
            foreach (var key in activeKeys) AgeKeyInfo(key);
            PruneKeyList();
            MouseWheel = 0;
            MouseDelta = new Coords(0, 0);
        }

        public Key GetKey(Keys keys) {
            return keyStates[(int)keys];
        }

        public IEnumerable<Keys> CurrentlyPressedKeys() {
            foreach (var key in activeKeys) if (key.IsPressed) yield return key.ID;
        }

        private void AgeKeyInfo(Key k) {
            if (k.status == Key.Status.Pressed) k.status = Key.Status.Held;
            if (k.status == Key.Status.Raised) k.status = Key.Status.Idle;
        }

        private void PrepareInput() {
        }

        internal void AcceptNewMousePosition(int x, int y) {
            var pos = new Coords(x, y);
            MouseDelta = pos - MousePosition;
            MousePosition = pos;
        }

        internal void SetKeyStatus(Keys key, Key.Status status) {
            var k = GetKey(key);
            k.status = status;
            activeKeys.Add(k);
        }

        internal void AcceptWheelDelta(float delta) {
            MouseWheel += delta;
        }

        internal void PruneKeyList() {
            activeKeys.RemoveWhere(k => k.status == Key.Status.Idle);
        }

        internal Keys Decode(SFML.Window.Keyboard.Key sfmlKey) {
            var index = (int)sfmlKey;
            if (index >= 0 && index < sfmlKeyMap.Length) return sfmlKeyMap[index];
            return Keys.NoKey;
        }

    }
}
