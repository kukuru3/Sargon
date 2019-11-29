namespace Sargon.Utils {
    public class Diagnostics {

        internal int SpritesDrawn { get; set; }
        internal int TextCharactersDrawn { get; set; }

        public int SpritesDrawnLastFrame { get; private set; }
        public int TextCharsDrawnLastFrame { get; private set; }


        internal void ResetFrame() {
            SpritesDrawnLastFrame = SpritesDrawn;
            TextCharsDrawnLastFrame = TextCharactersDrawn;
            SpritesDrawn = 0;
            TextCharactersDrawn = 0;
        }

        internal void FinishFrame() {

        }
    }
}
