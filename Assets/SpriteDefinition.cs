using System;
using System.Collections.Generic;
using System.Linq;
using Ur.Grid;
               
namespace Sargon.Assets {
    public class SpriteDefinition {
        
        public SpriteDefinition(string identity, Texture texture, Rect subrect, int sequence) {
            Texture     = texture;
            Rect        = subrect;
            Identity    = identity; 
            Sequence    = Sequence;
        }

        public Texture Texture  { get; }
        public Rect    Rect     { get; }
        public string  Identity { get; }
        public int     Sequence { get; }

        /// <summary> A helper operator, means we can supply an existing texture as a definition</summary>        
        public static implicit operator SpriteDefinition(Texture texture) => new SpriteDefinition("", texture, new Rect(0, 0, texture.Width, texture.Height), -1);
        public static implicit operator SpriteDefinition(string  textureID) => AssetManager.CurrentInstance.Find(textureID);
        
    }
}
