using System;
using System.Runtime.Serialization;

namespace VsTestLibrary
{
    public class VsTestLibraryException : Exception
    {
        public VsTestLibraryException()
        {
        }

        public VsTestLibraryException(string message) : base(message)
        {
        }

        public VsTestLibraryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VsTestLibraryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}