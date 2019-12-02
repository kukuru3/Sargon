using System;

namespace Sargon.Tweaks {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TweakAttribute : Attribute {

    }
}
