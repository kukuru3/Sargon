using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sargon.Graphics {
    public interface IRenderable : IDisposable {
        bool Visible { get; }
        Canvas OnCanvas { get; set; }
        void Display();
        float Zed { get; }
    }
}
