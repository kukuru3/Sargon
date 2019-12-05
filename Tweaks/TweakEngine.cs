using Sargon.Assets;
using Sargon.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sargon.Tweaks {
    public class TweakEngine : State {
        List<object> trackedTweakSourceObjects;
        List<ITrackedTweakable> trackedTweakables;
        Canvas canvas;
        bool tweakableSourcesDirty = false;
        Font font;

        ITweakChangeInterface Changer { get; set; }

        internal event Action TweakableListUpdated;

        protected internal override void Initialize() {
            Register(Hooks.Frame, Frame);
            trackedTweakSourceObjects = new List<object>();
            trackedTweakables = new List<ITrackedTweakable>();

            Context.StateManager.StateAdded += HandleStateAdded;
            Context.StateManager.StateRemoved += HandleStateRemoved;

            foreach (var state in Context.StateManager.ActiveStates) HandleStateAdded(state);

            tweakableSourcesDirty = true;
            var arr = Context.Assets.WithTag("sargon_debug_font").ToArray();

            var df = arr.FirstOrDefault() as Font;

            font =  df?? Context.Assets.DefaultFont;

            Changer = new SimpleTweakChangeInterface();
        }

        void Frame() {
            if (tweakableSourcesDirty) {
                TweakableListUpdated?.Invoke();
                RebuildRenderables();
                Changer.ProcessTweakableListUpdated(trackedTweakables);
                tweakableSourcesDirty = false;
            }

            Changer.Frame();

            UpdateTweakableValues();
        }

        private void UpdateTweakableValues() { 
            foreach (var t in trackedTweakables) t.Label.Content = t.Caption;
        }

        private void RebuildRenderables() {
            int x = 0;
            int y = 0;
            int h = 16;
            int tab = 15;

            if (canvas == null) canvas = new Canvas();
            canvas.Zed = 500f;
            canvas.Clear();

            foreach (var obj in trackedTweakSourceObjects) {
                var trackedTs = trackedTweakables.Where(t => t.Object == obj);
                if (trackedTs.Count() == 0) continue;

                var txt = new Text(PrintObjectName(obj), font);
                txt.Rect = Ur.Geometry.Rect.FromDimensions(x,y, 2000, h); y += h;
                txt.Color = new Ur.Color(1, 1, 0);
                canvas.Add(txt);

                foreach (var tweakable in trackedTs) {
                    var t = new Text(tweakable.Caption, font);
                    t.Rect = Ur.Geometry.Rect.FromBounds(x + tab, y, 2000, h); y += h;
                    t.Color = new Ur.Color(0.2f, 0.5f, 0, 1);
                    canvas.Add(t);
                    tweakable.Label = t;
                }
            }
        }

        string PrintObjectName(object obj) {
            return obj.GetType().FullName;
        }

        void HandleStateAdded(State state) {
            trackedTweakSourceObjects.Add(state);
            trackedTweakables.AddRange(FindProperties(state));
            tweakableSourcesDirty = true;
        }

        void HandleStateRemoved(State state) { 
            trackedTweakSourceObjects.Remove(state);
            trackedTweakables.RemoveAll(t => t.Object == state );
            tweakableSourcesDirty = true;
        }

        IEnumerable<ITrackedTweakable> FindProperties(object @object) {
            if (@object != null) { 
                var props = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var prop in props) {
                    var ta = prop.GetCustomAttribute<TweakAttribute>();
                    if (ta != null) {
                        yield return new TrackedTweakableProperty() {
                            attribute = ta,
                            Object = @object,
                            property = prop,
                        };
                    }
                }

                var fields = @object.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields) {
                    var ta = field.GetCustomAttribute<TweakAttribute>();
                    if (ta != null) {
                        yield return new TrackedTweakableField() {
                            attribute = ta,
                            Object = @object,
                            field = field,
                        };
                    }
                }
            }
        }

        internal interface ITrackedTweakable { 
            object Object { get; }
            string Caption { get; }
            Text Label { get; set; }
            Type Type { get; }

            void SetValue(object o);
            object GetValue();
        }

        class TrackedTweakableField : ITrackedTweakable {
            public string Caption => field.Name + ":" + GetValue();
            public object Object { get; set; }
            public Text Label { get; set; }
            public TweakAttribute attribute;
            public FieldInfo field;
            public Type Type => field.FieldType;

            public object GetValue() => field.GetValue(Object);
            public void SetValue(object value) => field.SetValue(Object, value);
        }

        class TrackedTweakableProperty : ITrackedTweakable {
            public string Caption => property.Name + ":" + GetValue();
            public object Object { get; set; }
            public Text Label { get; set; }
            public Type Type => property.PropertyType;

            public TweakAttribute attribute;
            public PropertyInfo property;

            public object GetValue() => property.GetValue(Object);
            public void SetValue(object value) => property.SetValue(Object, value);
        }
    }
}
