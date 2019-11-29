using System;

namespace Sargon.Graphics {
    public interface IRenderable : IDisposable {
        bool Visible { get; }
        Canvas OnCanvas { get; set; }
        void Display();
        float Zed { get; }
    }
}
