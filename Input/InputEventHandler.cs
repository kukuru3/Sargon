using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Sargon.Input {
    internal class InputEventHandler {
        Game Game { get; }
        public InputEventHandler(Game game) {
            Game = game;
        }

        internal void RegisterEvents(RenderWindow target) {

            target.TextEntered += TextEntered;
            target.KeyPressed += KeyPressed;
            target.KeyReleased += KeyReleased;
            target.MouseButtonPressed += MouseButtonPressed;
            target.MouseButtonReleased += MouseButtonReleased;
            target.MouseWheelScrolled += MouseWheelScrolled;
            target.MouseMoved += MouseMoved;
        }

        private void MouseMoved(object sender, SFML.Window.MouseMoveEventArgs e) {
            Game.Context.Input.AcceptNewMousePosition(e.X, e.Y);
        }

        private void MouseWheelScrolled(object sender, SFML.Window.MouseWheelScrollEventArgs e) {
            Game.Context.Input.AcceptWheelDelta(e.Delta);
        }

        private void MouseButtonReleased(object sender, SFML.Window.MouseButtonEventArgs e) {
            var key = Game.Context.Input.GetKey(Keys.Mouse1 + (int)e.Button);
            key.status = Key.Status.Raised;
        }

        private void MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e) {
            var key = Game.Context.Input.GetKey(Keys.Mouse1 + (int)e.Button);
            key.status = Key.Status.Pressed;
        }

        private void KeyReleased(object sender, SFML.Window.KeyEventArgs e) {
            var key = Game.Context.Input.Decode(e.Code);
            Game.Context.Input.GetKey(key).status = Key.Status.Raised;
        }

        private void KeyPressed(object sender, SFML.Window.KeyEventArgs e) {
            var key = Game.Context.Input.Decode(e.Code);
            Game.Context.Input.GetKey(key).status = Key.Status.Pressed;
        }

        private void TextEntered(object sender, SFML.Window.TextEventArgs e) {

        }
    }
}
