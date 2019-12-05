using Sargon.Assets;
using System;
using System.Collections.Generic;

namespace Sargon.Audio {
    public class AudioPlayer : State {

        private HashSet<Emitter> sounds; // these are emitters
        private HashSet<SoundInstance> instances; // these are helper objects that are a thin wrapper around SFML sounds or music.

        protected internal override void Initialize() {
            base.Initialize();
            instances = new HashSet<SoundInstance>();
            sounds = new HashSet<Emitter>();

            Register(Hooks.Frame, Update);
        }

        private void Update() { 
            CleanupInstances();
            foreach (var sound in sounds) sound.UpdateFrame();
        }

        public void Add(Emitter sound) {
            sounds.Add(sound);
        }

        public void Remove(Emitter sound) {
            sounds.Remove(sound);
            sound.DieGracefully();
        }

        public SoundInstance Play(Sample sample, bool loop = false, float volume = 1f) {
            if (sample == null) return null;
            SoundInstance item;
            if (sample.Streaming) {
                var m = new SFML.Audio.Music(sample.Path);
                m.Loop = loop;
                m.Volume = volume * 100f;
                m.Play();
                item = new SoundInstance() { music = m };
            } else {
                var s = new SFML.Audio.Sound();
                s.PlayingOffset = SFML.System.Time.FromSeconds(0f);
                s.SoundBuffer = sample.SoundBuffer;
                s.Loop = loop;
                s.Volume = volume * 100f;
                s.Play();
                item = new SoundInstance() { clip = s };
            }
            instances.Add(item);
            return item;
        }

        void CleanupInstances() {
            instances.RemoveWhere(item => item.IsComplete());
        }

        public class SoundInstance {
            internal SFML.Audio.Sound clip;
            internal SFML.Audio.Music music;

            internal bool IsComplete() => clip?.Status == SFML.Audio.SoundStatus.Stopped || music?.Status == SFML.Audio.SoundStatus.Stopped;
            internal void SetLooping(bool v) {
                if (clip != null) clip.Loop = v;
                if (music != null) music.Loop = v;
            }

            internal void SetPitch(float pitch) { 
                if (clip != null) clip.Pitch = pitch;
                if (music != null) music.Pitch = pitch;
            }

            internal void SetVolume(float realVolume) {
                if (clip != null) clip.Volume = realVolume * 100f;
                if (music != null) music.Volume = realVolume * 100f; 
            }

            internal void Stop() { 
                clip?.Stop();
                music?.Stop();
            }
        }
    }
}
