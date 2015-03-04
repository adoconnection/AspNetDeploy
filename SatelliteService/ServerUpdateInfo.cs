using System;
using System.Runtime.Serialization;

namespace SatelliteService
{
    [DataContract]
    public class ServerUpdateInfo
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime ReleaseDate { get; set; }

        [DataMember]
        public bool IsMandatory { get; set; }

        [DataMember]
        public bool IsDownloaded  { get; set; }
    }
}