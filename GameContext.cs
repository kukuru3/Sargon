using Sargon.Session;
namespace Sargon {
    public class GameContext {
        public   Input.Manager              Input    { get; internal set; }        
        public   Graphics.Pipeline          Pipeline { get; internal set; }
        
        internal Game GameInstance { get; }
        internal Input.InputEventHandler    InputHandler { get; }
        internal Utils.Logger               Logger       { get; }
        internal StateManager               StateManager { get; }
        internal BaseState                  BaseState    { get; set; }

        public Assets.AssetManager          Assets       { get; set; }

        static internal GameContext Current { get; private set; }
                                    
        public GameContext(Game sgame) {
            GameInstance = sgame;
            StateManager = new StateManager(sgame);
            InputHandler = new Input.InputEventHandler(sgame);           
            Logger       = new Utils.Logger();
            Assets       = new Assets.AssetManager();
            Current = this;
        }
    }
}
