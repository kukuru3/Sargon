using System;
using System.Collections.Generic;
using SFML.Window;
namespace Sargon {
    /// <summary> A basic Sargon game instance.</summary>
    public class Game {

        #region Public declarations
        public enum WindowStyle {
            Windowed,
            BorderlessWindowed,
            FullScreen
        }
        #endregion

        #region Fields
        private ResolutionMode currentMode; 
        #endregion

        #region Properties - public
        public string Title { get; set; }

        public int    TicksPerSecond {
            get { return Timer.GetTickFrequency(); }
            set { Timer.SetTickFrequency(value); }
        }

        /// <summary> Returns true if the game has STARTED running.</summary>
        public bool IsRunning { get; private set; } = false;

        public bool HasFocus { get; private set; } = true;

        /// <summary> Returns interpolation within one tick.</summary>
        public float Interpolation { get { return Timer.Interpolation;  } }
        #endregion

        #region Properties - private, internal
        private  SFML.Graphics.RenderWindow MainWindow { get; set;}
        public   Session.GameContext Context { get; private set; } 
        internal Session.GameTime Timer { get; }        
        #endregion

        #region Constructor
        public Game() {
            currentMode = new ResolutionMode(800, 600, WindowStyle.Windowed);
            Context = new Session.GameContext(this);            
            Timer = new Session.GameTime();
            CreateInternalStates();
        } 
        #endregion

        #region Public interface
        public void SetResolution(int width, int height, WindowStyle style) {
            this.currentMode = new ResolutionMode(width, height, style);
            if (MainWindow != null) {
                throw new NotImplementedException("Screen resize not implemented yet");
            }
        }

        public void AddState(State s) {
            Context.StateManager.AddState(s);
        }

        public void RemoveState(State s) {
            Context.StateManager.RemoveState(s);
        }
         
        /// <summary> Explicitly add a callback to one of the Sargon game hooks </summary>        
        public void AddCallback(Hooks atHook, Action toExecute, int priority = 0) {
            Context.BaseState.Register(atHook, toExecute);
        }

        public void Run() {
            
            CreateSFMLWindow();

            RegisterWindowEvents();
            
            SetupTimers();
            
            Context.StateManager.FlushStateQueues();

            Context.StateManager.Trigger(Hooks.Initialize);

            IsRunning = true;

            ExecuteMainLoop();

        }

        private void SetupTimers() {
            Timer.Start();
            MainWindow.SetFramerateLimit((uint)Timer.ScreenFramerateLimit);
            MainWindow.SetVerticalSyncEnabled(Timer.ScreenVSync);
        }

        public void Log(string str, ConsoleColor color = ConsoleColor.Gray) {
            Context.Logger.Add(str, color);
            Console.ForegroundColor = color;
            Console.WriteLine(str);
        }

        #endregion
        
        #region GUTS

        private void ExecuteMainLoop() {

            Timer.Ticked += ExecuteTick;

            while (MainWindow.IsOpen) {
                MainWindow.DispatchEvents();
                ExecuteFrame();
                Timer.Advance();                
            }
        }
           
        private void ExecuteFrame() {
            Context.StateManager.Trigger(Hooks.Frame);
            Context.StateManager.FlushStateQueues();
        }

        private void ExecuteTick() {
            Context.StateManager.Trigger(Hooks.Tick);
        }

        private void RegisterWindowEvents() {

            MainWindow.SetKeyRepeatEnabled(true);

            MainWindow.Closed += HandleClosed;
            MainWindow.LostFocus += HandleLostFocus;
            MainWindow.GainedFocus += HandleGainedFocus;
            
            Context.InputHandler.RegisterEvents(MainWindow);

        }
        
        private void CreateSFMLWindow() {
            if (MainWindow != null) {
                throw new InvalidOperationException("Cannot recreate SFML window once created!");
            }
            Styles sfmlStyles = 0;

            switch (currentMode.Style) {
                case WindowStyle.Windowed:
                    sfmlStyles = Styles.Titlebar | Styles.Close;
                    break;
                case WindowStyle.FullScreen:
                    sfmlStyles = Styles.Fullscreen;
                    break;
            }

            var sfmlContext = new ContextSettings(0, 0);
            MainWindow = new SFML.Graphics.RenderWindow(new VideoMode((uint)currentMode.Width, (uint)currentMode.Height), Title, sfmlStyles, sfmlContext);

        }

        private void CreateInternalStates() {

            Context.BaseState = new Session.BaseState();
            AddState(Context.BaseState);

            Context.Input = new Input.Manager();                    
            AddState(Context.Input);

            Context.Pipeline = new Graphics.Pipeline();
            AddState(Context.Pipeline);

            Context.StateManager.FlushStateQueues();

        } 
        
        private void Cleanup() {
            MainWindow.Closed       -= HandleClosed;
            MainWindow.LostFocus    -= HandleLostFocus;
            MainWindow.GainedFocus  -= HandleGainedFocus;
            Timer.Ticked            -= ExecuteTick;
        }

        #endregion

        #region Private event handlerss

        private void HandleClosed(object sender, EventArgs e) {
            Context.StateManager.Trigger(Hooks.End);            
            Cleanup();
            MainWindow.Close();
        }


        private void HandleGainedFocus(object sender, EventArgs e) {
            HasFocus = true;
        }

        private void HandleLostFocus(object sender, EventArgs e) {
            HasFocus = false;
            Context.Input.CancelKeys();
        }
        #endregion

        #region Private declarations
        private struct ResolutionMode {
            public int Width { get; }
            public int Height { get; }
            public WindowStyle Style { get; }

            public ResolutionMode(int width, int height, WindowStyle style) {
                Width = width;
                Height = height;
                Style = style;
            }
        } 
        #endregion

    }

        
    
    
}
