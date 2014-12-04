using System.Runtime.Serialization;

namespace SatelliteService
{
    [DataContract]
    public class ExceptionDataInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public bool IsProperty { get; set; }
    }
}