using System;
using System.Runtime.Serialization;

namespace SatelliteService
{
    public class SatelliteServiceException : Exception
    {
        public SatelliteServiceException()
        {
        }

        public SatelliteServiceException(string message) : base(message)
        {
        }

        public SatelliteServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SatelliteServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}