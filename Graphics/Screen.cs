namespace Sargon.Graphics {
    public class Screen {

        private Game GameInstance { get; }
        public Screen(Game sgame) {
            GameInstance = sgame;
        }

        public int Width => GameInstance.CurrentMode.Width;
        public int Height => GameInstance.CurrentMode.Height;

        public int HalfWidth => Width / 2;
        public int HalfHeight => Height / 2;

        public Ur.Grid.Coords Size => (Width, Height);

    }
}
