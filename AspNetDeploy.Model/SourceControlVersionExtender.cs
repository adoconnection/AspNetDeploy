using System;
using System.Linq;

namespace AspNetDeploy.Model
{
    public partial class SourceControlVersion
    {
        public DateTime GetDateTimeProperty(string key, DateTime defaultValue)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return defaultValue;
            }

            return DateTime.Parse(stringProperty);
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

        public int GetIntProperty(string key, int defaultValue = 0)
        {
            string stringProperty = this.GetStringProperty(key);

            if (string.IsNullOrEmpty(stringProperty))
            {
                return defaultValue;
            }

            return int.Parse(stringProperty);
        }

        public string GetStringProperty(string key, string defaultValue = null)
        {
            SourceControlVersionProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                return defaultValue;
            }

            return property.Value ?? defaultValue;
        }

        public void SetStringProperty(string key, string value)
        {
            SourceControlVersionProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                property = new SourceControlVersionProperty();
                property.SourceControlVersion = this;
                property.Key = key;
                this.Properties.Add(property);
            }

            property.Value = value;
        }
    }
}