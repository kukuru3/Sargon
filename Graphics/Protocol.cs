using System;
using System.Collections.Generic;

namespace Sargon.Graphics {
    public class Protocol {

        internal enum ProtocolItemTypes {
            None,
            BlitRenderTextureToTarget,
            ClearRenderTexture,
            FlushCanvasItems,
        }

        private struct ProtocolItem {
            internal ProtocolItemTypes         Type         { get; }            
            public SFML.Graphics.RenderTexture RenderSource { get; }
            public SFML.Graphics.RenderTarget  RenderTarget { get; }
            public SFML.Graphics.Color         Color        { get; }
            public int                         BlendMode    { get; }
            public Canvas                      Canvas       { get; }
            //public Shader                      Shader       { get; }

            public ProtocolItem(
                ProtocolItemTypes type,
                SFML.Graphics.RenderTexture renderSource = null,
                SFML.Graphics.RenderTarget  renderTarget = null,
                Ur.Color?                   color        = null,
                Canvas                      canvas       = null,
                //Dictionary<string, object>  shaderParams = null,
                int                         blendMode    = 0
            ) {
                this.Type = type;
                this.RenderSource = renderSource;
                this.RenderTarget = renderTarget;
                this.Color        = color?.ToSFMLColor() ?? SFML.Graphics.Color.Black;
                this.Canvas       = canvas;
                this.BlendMode    = blendMode;
                
            }
        }


    }
}
