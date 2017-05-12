using Newtonsoft.Json;

namespace AspNetDeploy.Notifications.Model
{
    public abstract class AppResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public dynamic Data { get; set; }
    }
}