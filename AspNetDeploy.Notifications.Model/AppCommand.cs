using System;

namespace AspNetDeploy.Notifications.Model
{
    public class AppCommand
    {
        public Guid UserGuid { get; set; }
        public string ConnectionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("data")]
        public dynamic Data;
    }
}