using System;
using System.Runtime.Serialization;

namespace Cesto
{
    /// <summary>
    /// Base class for exceptions thrown in this library.
    /// </summary>
    public class CestoException : Exception
    {
        #pragma warning disable 1591
        public CestoException() { }
        public CestoException(string message) : base(message) { }
        public CestoException(string message, Exception innerException) : base(message, innerException) { }
		protected CestoException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
