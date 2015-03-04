using System.Runtime.Serialization;

namespace SatelliteService
{
    [DataContract]
    public class DriveInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public double TotalSpaceMB { get; set; }

        [DataMember]
        public double FreeSpaceMB { get; set; }
    }
}