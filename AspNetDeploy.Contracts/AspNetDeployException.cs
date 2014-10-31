using System;
using System.Runtime.Serialization;

namespace AspNetDeploy.Contracts
{
    public class AspNetDeployException : Exception
    {
        public AspNetDeployException()
        {
        }

        public AspNetDeployException(string message) : base(message)
        {
        }

        public AspNetDeployException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AspNetDeployException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}