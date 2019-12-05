using System;
using System.Collections.Generic;

namespace Sargon.Tweaks {
    interface ITweakProcessor { 
        Type ProcessesType { get; }

        void Process(TweakEngine.ITrackedTweakable tweak);
    }


}
