namespace Sargon {
    public enum Hooks {

        /// <summary> Sargon initialized. Called only once.</summary>
        Initialize,

        /// <summary> Initial asset load requested </summary>
        Load,

        /// <summary> Fixed time stuff</summary>
        Tick,

        /// <summary> Per-frame stuff. Input should probably be handled here as well.</summary>
        Frame,

        /// <summary>When ever a window is resized or whatever</summary>
        WindowResolutionChanged,

        /// <summary>Sargon Cleanup started</summary>
        End

    }
}
