using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.Variables
{
    public class VariableProcessor : IVariableProcessor
    {
        private readonly Regex parseRegex = new Regex("{(?<type>var|env):(?<name>[^\\:}]+)(:(?<modifiers>[^}]+))*}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly IDictionary<string, string> dataFieldsDictionary;
        private readonly IDictionary<string, string> environmentDictionary;
        private readonly IDictionary<string, Func<string, string>> modifiersdictionary;

        public VariableProcessor(IDictionary<string, string> dataFieldsDictionary, IDictionary<string, string> environmentDictionary, IDictionary<string, Func<string, string>> modifiersdictionary)
        {
            this.dataFieldsDictionary = dataFieldsDictionary;
            this.environmentDictionary = environmentDictionary;
            this.modifiersdictionary = modifiersdictionary;
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
                
                IList<string> modifiers = new List<string>();

                if (!string.IsNullOrWhiteSpace(match.Groups["modifiers"].Value))
                {
                    modifiers = match.Groups["modifiers"].Value.Trim().Split(':').ToList();
                }

                nestedVariableNames.Add(variableName);

                if (variableType == "var")
                {
                    return this.ProcessMatch(nestedVariableNames, variableName, match.Value, this.dataFieldsDictionary, this.modifiersdictionary, modifiers);
                }
                
                if (variableType == "env")
                {
                    return this.ProcessMatch(nestedVariableNames, variableName, match.Value, this.environmentDictionary, this.modifiersdictionary, modifiers);
                }

                return match.Value;
            });
        }

        private string ProcessMatch(ICollection<string> nestedVariableNames, string variableName, string defaultValue, IDictionary<string, string> variableDictionary, IDictionary<string, Func<string, string>> modifiersdictionary, IList<string> modifiers)
        {
            string key = variableDictionary.Keys.FirstOrDefault(k => k.ToLowerInvariant() == variableName);

            string value = defaultValue;

            if (key != null)
            {
                value = this.ProcessValueInternal(variableDictionary[key], nestedVariableNames);
            }

            foreach (string modifier in modifiers)
            {
                if (modifiersdictionary.ContainsKey(modifier.ToLowerInvariant()))
                {
                    value = modifiersdictionary[modifier.ToLowerInvariant()](value);
                }
            }

            return value;
        }
    }
}