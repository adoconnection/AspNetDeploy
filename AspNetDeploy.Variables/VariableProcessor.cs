using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.Variables
{
    public class VariableProcessor : IVariableProcessor
    {
        private readonly Regex parseRegex = new Regex("{(?<type>var|env):(?<name>[^}]+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly IDictionary<string, string> dataFieldsDictionary;
        private readonly IDictionary<string, string> environmentDictionary;

        public VariableProcessor(IDictionary<string, string> dataFieldsDictionary, IDictionary<string, string> environmentDictionary)
        {
            this.dataFieldsDictionary = dataFieldsDictionary;
            this.environmentDictionary = environmentDictionary;
        }

        public string ProcessValue(string value)
        {
            return this.ProcessValueInternal(value, new List<string>());
        }

        private string ProcessValueInternal(string value, ICollection<string> nestedVariableNames)
        {
            if (value == null)
            {
                return null;
            }

            return parseRegex.Replace(value, delegate(Match match)
            {
                string variableName = match.Groups["name"].Value.ToLowerInvariant();
                string variableType = match.Groups["type"].Value.ToLowerInvariant();

                nestedVariableNames.Add(variableName);

                if (variableType == "var")
                {
                    return this.ProcessMatch(nestedVariableNames, variableName, match.Value, this.dataFieldsDictionary);
                }
                
                if (variableType == "env")
                {
                    return this.ProcessMatch(nestedVariableNames, variableName, match.Value, this.environmentDictionary);
                }

                return match.Value;
            });
        }

        private string ProcessMatch(ICollection<string> nestedVariableNames, string variableName, string defaultValue, IDictionary<string, string> variableDictionary)
        {
            string key = variableDictionary.Keys.FirstOrDefault(k => k.ToLowerInvariant() == variableName);

            if (key != null)
            {
                return this.ProcessValueInternal(variableDictionary[key], nestedVariableNames);
            }

            return defaultValue;
        }
    }
}