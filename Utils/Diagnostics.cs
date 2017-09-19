using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sargon.Utils {
    public class Diagnostics {

        public int SpritesDrawn { get; internal set; }
        public int TextCharactersDrawn { get; internal set; }

        internal void ResetFrame() {
            SpritesDrawn = 0;
            TextCharactersDrawn = 0;
        }

        internal void FinishFrame() {
            
        }
    }
}
