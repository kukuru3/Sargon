using Ur.Grid;

using Vector2 = Ur.Geometry.Vector2;

namespace Sargon.Assets {
    public class SpriteDefinition {

        public SpriteDefinition(string identity, Texture texture, Rect subrect, int sequence, Vector2? explicitAnchor = null) {
            Texture = texture;
            Rect = subrect;
            Identity = identity;
            Sequence = Sequence;
            Anchor = explicitAnchor ?? new Vector2(0.5f, 0.5f);
        }

        public Texture Texture { get; }
        public Rect Rect { get; }
        public string Identity { get; }
        public int Sequence { get; }

        public Vector2 Anchor { get; }

        /// <summary> A helper operator, means we can supply an existing texture as a definition </summary>
        public static implicit operator SpriteDefinition(Texture texture) => new SpriteDefinition("", texture, new Ur.Grid.Rect(0, 0, texture.Width, texture.Height), -1);
        public static implicit operator SpriteDefinition(string textureID) => GameContext.Current.Assets.Find(textureID);

    }
}
