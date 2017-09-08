using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sargon.Errors {

    [Serializable]
    public class SargonException : Exception {
        public SargonException() { }
        public SargonException(string message) : base(message) { }
        public SargonException(string message, Exception inner) : base(message, inner) { }
        protected SargonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
