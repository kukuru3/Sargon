using System;
using System.Collections.Generic;
using System.Linq;
using Ur.Grid;
               
namespace Sargon.Assets {
    public class SpriteDefinition {
        
        public SpriteDefinition(string identity, Texture texture, Rect subrect, int sequence, Coords? explicitAnchor = null) {
            Texture     = texture;
            Rect        = subrect;
            Identity    = identity; 
            Sequence    = Sequence;
            Anchor      = explicitAnchor ?? new Coords(Rect.Width / 2, Rect.Height / 2);
        }

        public Texture Texture  { get; }
        public Rect    Rect     { get; }
        public string  Identity { get; }
        public int     Sequence { get; }
        public Coords  Anchor   { get; }

        /// <summary> A helper operator, means we can supply an existing texture as a definition</summary>        
        public static implicit operator SpriteDefinition(Texture texture) => new SpriteDefinition("", texture, new Rect(0, 0, texture.Width, texture.Height), -1);
        public static implicit operator SpriteDefinition(string  textureID) => GameContext.Current.Assets.Find(textureID);
        
    }
}
