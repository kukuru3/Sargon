using System;

namespace Sargon {
    public abstract class State {

        internal Session.StateManager Manager { get; set; }

        public bool IsInternal { get; internal set; }

        public Game Game => Manager.GameInstance;
        public GameContext Context => Manager.GameInstance.Context;

        public virtual string Name => GetType().FullName;

        protected internal void Register(Hooks hook, Action a, int priority = 0) {
            Manager.RegisterHook(hook, a, priority);
        }

        /// <summary> Explicitly unregister a single previous action. </summary>
        protected internal void Unregister(Hooks hook, Action a) {
            Manager.UnregisterHook(hook, a);
        }

        /// <summary> Called by Sargon after all hooks have been unregistered.
        /// There is no need to remove your hooks explicitly here.</summary>
        protected internal virtual void Cleanup() { }

        /// <summary> Called by Sargon when state is initialized. 
        /// Hooks must be registered here.</summary>
        protected internal virtual void Initialize() { }

        /// <summary> Will remove this state from the game</summary>
        protected void RemoveSelf() {
            Manager.RemoveState(this);
        }
    }
}
