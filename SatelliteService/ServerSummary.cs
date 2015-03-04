using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SatelliteService
{
    [DataContract]
    public class ServerSummary
    {
        [DataMember]
        public List<DriveInfo> Drives { get; set; }

        [DataMember]
        public List<ServerUpdateInfo> Updates { get; set; }

        [DataMember]
        public double AvailableMemoryMB { get; set; }

        [DataMember]
        public double TotalMemoryMB { get; set; }
    }
}