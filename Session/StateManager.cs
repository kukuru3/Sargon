using System;
using System.Collections.Generic;
using System.Linq;
using Ur;

namespace Sargon.Session {
    internal class StateManager {

        #region Private fields
        Dictionary<Hooks, List<Action>> callList;
        List<MethodDef> allMethods;
        Queue<State> addedStates;
        Queue<State> removedStates;
        List<State> activeStates;

        internal Game GameInstance { get; }

        internal event Action<State> StateAdded;
        internal event Action<State> StateRemoved;

        #endregion

        #region Ctor
        public StateManager(Game gameInstance) {
            allMethods = new List<MethodDef>();
            addedStates = new Queue<State>();
            removedStates = new Queue<State>();
            activeStates = new List<State>();
            GameInstance = gameInstance;
        }
        #endregion

        #region State addition and removal

        internal IEnumerable<State> ActiveStates => activeStates;

        internal void FlushStateQueues() {
            while (addedStates.Count > 0) DoAddState(addedStates.Dequeue());
            while (removedStates.Count > 0) DoRemoveState(removedStates.Dequeue());
        }

        private void DoAddState(State state) {
            state.Manager = this;
            state.Initialize();
            StateAdded?.Invoke(state);
            activeStates.Add(state);
            GameInstance.Context.Logger.Add("initializing state : " + state.Name, ConsoleColor.DarkGreen);
        }

        private void DoRemoveState(State state) {
            allMethods.RemoveAll(m => m.Method.Target == state);
            InvalidateCallList();
            StateRemoved?.Invoke(state);
            activeStates.Remove(state);
            state.Cleanup();
        }

        public void AddState(State state) {
            if (addedStates.Contains(state)) return;
            addedStates.Enqueue(state);
        }

        public void RemoveState(State state) {
            if (removedStates.Contains(state)) return;
            removedStates.Enqueue(state);
        }

        #endregion

        #region Hook trigger and maintenance

        internal void Trigger(Hooks hook) {
            if (callList == null) RebuildCallList();
            foreach (var registeredCall in callList[hook]) registeredCall.Invoke();
        }

        internal void RegisterHook(Hooks hook, Action callThis, int priority = 0) {
            var md = new MethodDef(hook, callThis, priority);
            allMethods.Add(md);
            InvalidateCallList();
        }

        internal void UnregisterHook(Hooks hook, Action a) {
            allMethods.RemoveAll(m => m.Hook == hook && m.Method == a);
            InvalidateCallList();
        }

        #endregion

        #region Call list management

        void InvalidateCallList() {
            callList = null;
        }

        void RebuildCallList() {
            callList = new Dictionary<Hooks, List<Action>>();
            var tempList = new Dictionary<Hooks, List<MethodDef>>();

            foreach (var hook in Enums.IterateValues<Hooks>()) {
                tempList.Add(hook, new List<MethodDef>());
            }

            foreach (var item in allMethods) tempList[item.Hook].Add(item);
            foreach (var key in tempList.Keys) {
                tempList[key].Sort((a, b) => Math.Sign(a.Priority - b.Priority));
                callList[key] = tempList[key].Select(md => md.Method).ToList();
            }

        }

        #endregion

        #region Helper structures definition
        private struct MethodDef {
            internal readonly Action Method;
            internal readonly Hooks Hook;
            internal readonly int Priority;
            public MethodDef(Hooks hook, Action method, int priority) {
                Hook = hook;
                Method = method;
                Priority = priority;
            }
        }
        #endregion
    }
}
