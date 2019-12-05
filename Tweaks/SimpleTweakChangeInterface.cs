using System;
using System.Collections.Generic;
using System.Linq;
using Sargon.Input;

using ITweakable = Sargon.Tweaks.TweakEngine.ITrackedTweakable;

namespace Sargon.Tweaks {

    interface ITweakChangeInterface {
        void Frame();
        void ProcessTweakableListUpdated(IEnumerable<TweakEngine.ITrackedTweakable> tweakables);
        ITweakable FocusedTweakable { get; }
    }

    // A "tweak changer" is a component that manages user inputs and uses them to change tweakable values.
    // in the current implementation, it defers the actual task to an instance of TweakProcessor
    // It also maintains and changes the "current tweakable"
    class SimpleTweakChangeInterface : ITweakChangeInterface {

        List<ITweakable> tweakables;
        ITweakProcessor CurrentProcessor { get; set; }
        Dictionary<Type, ITweakProcessor> registeredProcessors;

        // the Tweakable that is currently being edited.
        public ITweakable FocusedTweakable { get; private set; }

        public SimpleTweakChangeInterface() {
            RegisterProcessors();
            tweakables = new List<ITweakable>();
        }

        public void Frame() {
            ProcessTweakableSelection();
            if (FocusedTweakable != null) CurrentProcessor?.Process(FocusedTweakable);
        }

        private void ProcessTweakableSelection() {
            
            if (Keys.LCtrl.IsHeld()) { 
                if (Keys.F11.IsPressed()) CycleFocus(-1);
                if (Keys.F12.IsPressed()) CycleFocus(1);
            }
        }

        private void CycleFocus(int delta) {
            var t = GetElementOffsetFromFocusByThiManyMembers(delta);
            SetFocus(t);
        }

        private ITweakable GetElementOffsetFromFocusByThiManyMembers(int delta) {
            var index = tweakables.IndexOf(FocusedTweakable);
            if (index == -1) return tweakables.FirstOrDefault();
            index += delta;
            if (index < 0) return tweakables.Last();
            else if (index >= tweakables.Count) return tweakables.First();
            else return tweakables[index];
        }

        public void ProcessTweakableListUpdated(IEnumerable<ITweakable> sourceList) {
            tweakables = new List<ITweakable>(sourceList);
            if (!tweakables.Contains(FocusedTweakable)) SetFocus(null);
        }

        void SetFocus(ITweakable tweakable) {
            
            if (FocusedTweakable?.Label != null) FocusedTweakable.Label.Color = new Ur.Color(0.2f, 0.5f, 0, 1);
            FocusedTweakable = tweakable;
            CurrentProcessor = TryGetProcessorForTweakable(FocusedTweakable);
            if (FocusedTweakable?.Label != null) FocusedTweakable.Label.Color = (CurrentProcessor == null) ? new Ur.Color(0.6f, 0f, 0f, 1) : new Ur.Color(0.6f, 0.5f, 0.2f, 1);
        }

        private ITweakProcessor TryGetProcessorForTweakable(ITweakable focusedTweakable) {
            if (focusedTweakable == null) return null;
            registeredProcessors.TryGetValue(focusedTweakable.Type, out var result);
            return result;
        }

        private void RegisterProcessors() {
            registeredProcessors = new Dictionary<Type, ITweakProcessor>();
            var allprocessorTypes = Ur.Typesystem.Finder.FromAllAssemblies().ImplementingTypes<ITweakProcessor>();
            var allProcessors = allprocessorTypes.Select(t => Activator.CreateInstance(t) as ITweakProcessor);
            foreach (var p in allProcessors) registeredProcessors.Add(p.ProcessesType, p);
        }
    }
}
