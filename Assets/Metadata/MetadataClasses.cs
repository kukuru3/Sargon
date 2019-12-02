using System.Collections.Generic;

namespace Sargon.Assets.Metadata {
    public abstract class BaseMetadata {
        public BaseMetadata() { tags = new List<string>(); }
        public List<string> tags { get; set; }
    }

    public class SampleMetadata : BaseMetadata {
        public bool streamed { get; set; }
    }

    public class TextureMetadata : BaseMetadata {
        public Dictionary<string, int[]> sprites { get; set; }
        public GridDef grid { get; set; }

        public class GridDef {
            public int chars_w { get; set; }
            public int chars_h { get; set; }
            public string[][] rows { get; set; }
        }
    }

    public class FontMetadata : BaseMetadata {
        public int DefaultSize { get; set; }
    }

    public class ShaderMetadata : BaseMetadata {

    }
}
