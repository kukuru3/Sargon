using System;
using Sargon.Assets;
using Ur;

namespace Sargon.Audio {
    /// <summary> Our version of an audio emitter, or audio source.</summary>
    public class Emitter {
        public bool Looping { get; set; }

        public float Volume { get; set; } = 1f; // actually a target.
        public Sample Sample { get; set; } // also actually a target

        float RealVolume { get; set; }

        Sample ActualSample { get; set; }

        public float SmoothTime { get; set; } = 0.1f; // if > 0, will smooth out transitions

        public float Pitch { get; set; } = 1.0f;

        public float Pan { get; set; } = 0f;

        float phaseMultiplier = 0f;

        AudioPlayer.SoundInstance currentInstance;

        internal void UpdateFrame() {
            UpdateTransitions();
        }

        private void UpdateTransitions() {
            if (markedForDeath) Sample = null;

            var doPhaseOut = (ActualSample != Sample);
            var targetPhaseOutMultiplier = doPhaseOut ? 0f : 1f;
            phaseMultiplier = phaseMultiplier.Approach(targetPhaseOutMultiplier, GameContext.Current.Timer.FrameTime / SmoothTime);

            if (doPhaseOut && phaseMultiplier.Approximately(0f)) {
                currentInstance?.Stop();
                ActualSample = Sample;
                currentInstance = GameContext.Current.Audio.Play(Sample, Looping);
            }

            RealVolume = RealVolume.Approach(Volume, GameContext.Current.Timer.FrameTime / SmoothTime);
            currentInstance?.SetVolume(RealVolume * phaseMultiplier);

            currentInstance?.SetPitch(Pitch);
        }

        bool markedForDeath = false;

        internal void DieGracefully() {
            Sample = null;
            markedForDeath = true;
            currentInstance.SetLooping(false);
        }
    }
}
