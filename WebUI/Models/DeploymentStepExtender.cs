using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebUI.Models
{
    public partial class DeploymentStep
    {
        public int GetIntProperty(string key, int defaultValue)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return defaultValue;
            }

            return int.Parse(stringProperty);
        }

        public dynamic GetDynamicProperty(string key)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(stringProperty);
        }

        public IList<dynamic> GetDynamicProperties(string key)
        {
            return this.GetStringProperties(key).Select(JsonConvert.DeserializeObject).ToList();
        }

        public string GetStringProperty(string key, string defaultValue = null)
        {
            DeploymentStepProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                return defaultValue;
            }

            return property.Value ?? defaultValue;
        }

        public IList<string> GetStringProperties(string key)
        {
            return this.Properties.Where(p => p.Key.ToLower() == key.ToLower()).Select( p => p.Value).ToList();
        }
    }
}