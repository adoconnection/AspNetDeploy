using System;
using System.Runtime.Serialization;
using AspNetDeploy.Contracts.Exceptions;

namespace BuildServices.Gulp
{
    public class GulpException : AspNetDeployException
    {
        public GulpException()
        {
        }

        public GulpException(string message) : base(message)
        {
        }

        public GulpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GulpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}