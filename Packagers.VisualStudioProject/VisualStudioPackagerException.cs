using System;
using System.Runtime.Serialization;
using AspNetDeploy.Contracts.Exceptions;

namespace Packagers.VisualStudioProject
{
    public class VisualStudioPackagerException : AspNetDeployException
    {
        public VisualStudioPackagerException()
        {
        }

        public VisualStudioPackagerException(string message) : base(message)
        {
        }

        public VisualStudioPackagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VisualStudioPackagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}