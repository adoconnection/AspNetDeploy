using System;
using System.Runtime.Serialization;
using AspNetDeploy.Contracts.Exceptions;

namespace AspNetDeploy.BuildServices.DotnetCore
{
    public class DotnetCoreBuildServiceException : AspNetDeployException
    {
        public DotnetCoreBuildServiceException()
        {
        }

        public DotnetCoreBuildServiceException(string message) : base(message)
        {
        }

        public DotnetCoreBuildServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DotnetCoreBuildServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}