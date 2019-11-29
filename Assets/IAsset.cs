using System;

namespace Sargon.Assets {

    public enum LoadStates {
        /// <summary> Loading was not initiated</summary>
        NotLoaded,
        /// <summary> Loading in progress.</summary>
        Loading,
        /// <summary> Loaded and active.</summary>
        Active,
        /// <summary> Loading was attempted and failed.</summary>
        Failed,
    }

    public interface IAsset : IDisposable {

        string Path { get; }
        LoadStates LoadState { get; }
        void StartLoad();
        void Unload();


    }
}
