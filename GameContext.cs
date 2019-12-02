using Sargon.Session;
namespace Sargon {
    public class GameContext {

        public Input.Manager Input { get; internal set; }
        public Graphics.Pipeline Pipeline { get; internal set; }
        public Utils.Diagnostics Diagnostics { get; internal set; }

        public Game GameInstance { get; }
        public Utils.Logger Logger { get; }

        internal Input.InputEventHandler InputHandler { get; }
        internal StateManager StateManager { get; }
        internal Graphics.Renderer Renderer { get; }
        internal BaseState BaseState { get; set; }

        public Graphics.Screen Screen { get; }

        public Audio.AudioPlayer Audio { get; internal set; }

        public Assets.AssetDatabase Assets { get; set; }

        static internal GameContext Current { get; private set; }

        public GameTime Timer => GameInstance.Timer;

        public GameContext(Game sgame) {
            GameInstance = sgame;
            StateManager = new StateManager(sgame);
            InputHandler = new Input.InputEventHandler(sgame);
            Diagnostics = new Utils.Diagnostics();
            Logger = new Utils.Logger();
            Assets = new Assets.AssetDatabase();
            Renderer = new Graphics.Renderer();
            Screen = new Graphics.Screen(sgame);

            Current = this;
        }
    }
}
