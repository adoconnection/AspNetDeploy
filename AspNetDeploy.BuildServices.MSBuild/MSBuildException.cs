using System;
using System.Runtime.Serialization;
using AspNetDeploy.Contracts.Exceptions;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildException : AspNetDeployException
    {
        public MSBuildException()
        {
        }

        public MSBuildException(string message) : base(message)
        {
        }

        public MSBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MSBuildException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}