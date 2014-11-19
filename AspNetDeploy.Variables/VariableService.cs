using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.Variables
{
    public class VariableService : IVariableService
    {
        private readonly Regex parseRegex = new Regex("{var:(?<name>[^}]+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private List<DataField> dataFields;

        private int machineId;
        private int environMentId;

        public void Configure(int bundleId, int machineId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            this.dataFields = entities.DataField
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .ToList();

            this.machineId = machineId;
            this.machineId = entities.Machine.First( m => m.Id == machineId).Environments.First().Id;
        }

        public string ProcessValue(string value)
        {
            return parseRegex.Replace(value, delegate(Match match)
            {
                string variableName = match.Groups["name"].Value.ToLowerInvariant();

                DataField dataField = this.dataFields.FirstOrDefault( df => df.Key == variableName);

                if (dataField == null)
                {
                    return match.Value;
                }

                DataFieldValue machineValue = dataField.DataFieldValues.FirstOrDefault( dfv => dfv.Machines.Any( e => e.Id == environMentId));

                if (machineValue != null)
                {
                    return machineValue.Value;
                }

                DataFieldValue environmentValue = dataField.DataFieldValues.FirstOrDefault( dfv => dfv.Environments.Any( e => e.Id == environMentId));

                if (environmentValue != null)
                {
                    return environmentValue.Value;
                }

                return match.Value;
            });
        }
    }
}