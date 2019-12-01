using System;
using System.Collections.Generic;
using Sargon.Assets;
using Ur.Geometry;

namespace Sargon.Graphics {

    public interface IHasEffect {
        Effect Effect { get; }
    }

    // A wrapper for a shader and its parameters.
    public class Effect {

        Dictionary<string, object> parameterBag;

        public Effect(string shaderID) {
            Shader = GameContext.Current.Assets.GetShader(shaderID);
            parameterBag = new Dictionary<string, object>();
        }

        public Effect(Shader s) {
            Shader = s;
            parameterBag = new Dictionary<string, object>();
        }

        public Shader Shader { get; }

        public void AddParam(string id, float param) => parameterBag[id] = param;
        public void AddParam(string id, Ur.Color param) => parameterBag[id] = param;
        public void AddParam(string id, Vector2 param) => parameterBag[id] = param;
        public void AddParam(string id, Texture param) => parameterBag[id] = param;

        internal void Apply() {
            var ns = Shader?.NativeShader;
            if (ns == null) return;

            foreach (var kvp in parameterBag) {
                var id = kvp.Key;
                switch (kvp.Value) {
                    case float f: ns.SetUniform(id, f); break;
                    case Vector2 v: ns.SetUniform(id, v.ToSFMLVector2f()); break;
                    case Ur.Color c: ns.SetUniform(id, new SFML.Graphics.Glsl.Vec4(c.r, c.g, c.b, c.a)); break;
                    case Texture t: ns.SetUniform(id, t.NativeTexture); break;
                    default: throw new Exception($"Don't know how to handle effect parameter type: {kvp.Value?.GetType()?.FullName}");
                }
            }
        }
    }
}
