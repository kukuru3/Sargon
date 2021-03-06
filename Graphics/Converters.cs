﻿namespace Sargon.Graphics {
    static public class ConversionUtility {

        static internal SFML.Graphics.Color ToSFMLColor(this Ur.Color source) {
            var z = Ur.Color.ToUnsignedInteger(source);
            return new SFML.Graphics.Color(z);
        }

        static internal SFML.System.Vector2f ToSFMLVector2f(this Ur.Geometry.Vector2 source) {
            return new SFML.System.Vector2f(source.x, source.y);
        }

        static internal SFML.Graphics.IntRect ToSFMLIntRect(this Ur.Grid.Rect source) {
            return new SFML.Graphics.IntRect(source.X0, source.Y0, source.Width, source.Height);
        }

    }
}
