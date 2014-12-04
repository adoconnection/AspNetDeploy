using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SatelliteService
{
    [DataContract]
    public class ExceptionInfo
    {
        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public string AssemblyQualifiedTypeName { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public List<ExceptionDataInfo> ExceptionData { get; set; }

        [DataMember]
        public ExceptionInfo InnerException { get; set; }
    }
}