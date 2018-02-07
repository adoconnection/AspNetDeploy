using System;
using System.Runtime.Serialization;
using AspNetDeploy.Contracts.Exceptions;

namespace AspNetDeploy.BuildServices.NuGet
{
    public class NugetException : AspNetDeployException
    {
        public NugetException()
        {
        }

        public NugetException(string message) : base(message)
        {
        }

        public NugetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NugetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}