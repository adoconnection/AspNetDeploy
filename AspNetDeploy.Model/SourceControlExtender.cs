using System.Linq;

namespace AspNetDeploy.Model
{
    public partial class SourceControl
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

        public string GetStringProperty(string key, string defaultValue = null)
        {
            SourceControlProperty property = this.Properties.FirstOrDefault( p => p.Key.ToLower() == key.ToLower());

            if (property == null)
            {
                return defaultValue;
            }

            return property.Value ?? defaultValue;
        }
    }
}