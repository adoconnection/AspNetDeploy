using System.Collections.Generic;
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

        private string ProcessValueInternal(string value, IList<string> nestedVariableNames)
        {
            return parseRegex.Replace(value, delegate(Match match)
            {
                string variableName = match.Groups["name"].Value.ToLowerInvariant();
                string variableType = match.Groups["type"].Value.ToLowerInvariant();
                nestedVariableNames.Add(variableName);

                if (variableType == "var")
                {
                    if (this.dataFieldsDictionary.ContainsKey(variableName))
                    {
                        return this.ProcessValueInternal(this.dataFieldsDictionary[variableName], nestedVariableNames);
                    }
                    
                    return match.Value;
                }
                
                if (variableType == "evn")
                {
                    if (this.environmentDictionary.ContainsKey(variableName))
                    {
                        return this.ProcessValueInternal(this.environmentDictionary[variableName], nestedVariableNames);
                    }
                    
                    return match.Value;
                }

                return match.Value;
            });
        }
    }
}