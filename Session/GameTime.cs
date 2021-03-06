﻿using System;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Sargon.Session {
    public class GameTime {

        const int DEFAULT_TICK_FREQUENCY = 20;
        const int MAX_TICKS_PER_LOOP = 5;
        const int DEFAULT_FRAMERATE = 60;
        const bool DEFAULT_VSYNC = false;

        private Stopwatch Stopwatch;
        private int tickFrequency = DEFAULT_TICK_FREQUENCY;
        private long SystemTicksPerGameTick = TimeSpan.TicksPerSecond / DEFAULT_TICK_FREQUENCY;
        private TimeSpan timeOfPreviousTick;

        public int ScreenFramerateLimit { get; private set; } = DEFAULT_FRAMERATE;
        public bool ScreenVSync { get; private set; } = DEFAULT_VSYNC;

        public float ElapsedTime => (float)Stopwatch.Elapsed.TotalSeconds;
        public float Interpolation { get; private set; }

        internal long FrameCounter { get; private set; }
        internal long TickCounter { get; private set; }

        internal void SetTickFrequency(int ticks) {
            this.tickFrequency = ticks;
            SystemTicksPerGameTick = (TimeSpan.TicksPerSecond / ticks);
        }

        internal int GetTickFrequency() {
            return this.tickFrequency;
        }

        public GameTime() {
            Stopwatch = new Stopwatch();
        }

        public void Start() {
            timeOfPreviousTick = TimeSpan.Zero;
            FrameCounter = 0;
            Stopwatch.Start();
        }

        public event Action Ticked;

        void ExecuteTick() {
            Ticked?.Invoke();
        }

        public void Advance() {
            var elapsedTime = Stopwatch.Elapsed;
            var loopCounter = 0;
            while ((elapsedTime - timeOfPreviousTick).Ticks > SystemTicksPerGameTick) {
                timeOfPreviousTick += new TimeSpan(SystemTicksPerGameTick);
                loopCounter++;
                TickCounter++;
                ExecuteTick();
                if (loopCounter > MAX_TICKS_PER_LOOP) break;
            }
            FrameCounter++;
            Interpolation = Ur.Numbers.Choke(
                (float)(elapsedTime - timeOfPreviousTick).Ticks / SystemTicksPerGameTick, 0f, 1f
            );

        }

        private double recordedFrameTime;

        internal void CaptureFrameTime() {
            var el = Stopwatch.Elapsed.TotalSeconds;
            FrameTime = (float)(el - recordedFrameTime);
            recordedFrameTime = el;
        }

        public float FrameTime { get; private set; }
    }
}
