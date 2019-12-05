using System;
using Sargon.Input;

namespace Sargon.Tweaks {
    //class IntTweakProcessor : ITweakProcessor {
    //    public Type ProcessesType => typeof(int);

    //    public void Process()  { }
    //}

    class FloatTweakProcessor : ITweakProcessor {
        public Type ProcessesType => typeof(float);

        public void Process(TweakEngine.ITrackedTweakable tweakable) {
            if (Keys.RightBracket.IsPressed()) OffsetValue(tweakable, 0.1f);
            if (Keys.LeftBracket.IsPressed()) OffsetValue(tweakable, -0.1f);
        }

        private static void OffsetValue(TweakEngine.ITrackedTweakable tweakable, float delta) {
            var value = (float)tweakable.GetValue();
            value += delta;
            tweakable.SetValue(value);
        }
    }


}
