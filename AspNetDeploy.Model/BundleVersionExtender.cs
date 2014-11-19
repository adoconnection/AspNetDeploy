using System;
using System.Linq;

namespace AspNetDeploy.Model
{
    public partial class BundleVersion
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
            BundleVersionProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                return defaultValue;
            }

            return property.Value ?? defaultValue;
        }

        public void SetStringProperty(string key, string value)
        {
            BundleVersionProperty property = this.Properties.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                property = new BundleVersionProperty();
                property.BundleVersion = this;
                property.Key = key;
                this.Properties.Add(property);
            }

            property.Value = value;
        }
    }
}