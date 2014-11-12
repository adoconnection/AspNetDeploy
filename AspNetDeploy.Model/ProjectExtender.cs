using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AspNetDeploy.Model
{
    public partial class Project
    {
        public int GetIntProperty(string key, int defaultValue = 0)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return defaultValue;
            }

            return int.Parse(stringProperty);
        }

        public bool GetBoolProperty(string key, bool defaultValue = false)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return defaultValue;
            }

            return bool.Parse(stringProperty);
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
            ProjectProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                return defaultValue;
            }

            return property.Value ?? defaultValue;
        }

        public IList<string> GetStringProperties(string key)
        {
            return this.Properties.Where(p => p.Key.ToLower() == key.ToLower()).Select(p => p.Value).ToList();
        }

        public void SetBoolProperty(string key, bool value)
        {
            this.SetStringProperty(key, value.ToString());
        }

        public void SetStringProperty(string key, string value)
        {
            ProjectProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                property = new ProjectProperty();
                property.Project = this;
                property.Key = key;
                this.Properties.Add(property);
            }

            property.Value = value;
        }
    }
}