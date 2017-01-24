using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.Variables
{
    public class VariableProcessorFactory : IVariableProcessorFactory
    {
        public IVariableProcessor Create(int bundleVersionId, int machineId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            List<DataField> dataFields = entities.DataField
                .Include("DataFieldValues.Environments")
                .Include("DataFieldValues.Machines")
                .Where( df => !df.IsDeleted)
                .ToList();

            BundleVersion bundleVersion = entities.BundleVersion
                .Include("Bundle")
                .Include("ParentBundleVersion")
                .First(bv => bv.Id == bundleVersionId);

            int environmentId = entities.Machine.First(m => m.Id == machineId).Environments.First().Id;

            IDictionary<string, string> dataFieldsDictionary = CreateDataFieldsDictionary(machineId, dataFields, environmentId);
            IDictionary<string, string> environmentDictionary = this.CreateEnvironmentDictionary(bundleVersion);

            return new VariableProcessor(dataFieldsDictionary, environmentDictionary);
        }

        private static IDictionary<string, string> CreateDataFieldsDictionary(int machineId, IEnumerable<DataField> dataFields, int environmentId)
        {
            IDictionary<string, string> dataFieldsDictionary = new Dictionary<string, string>();

            foreach (DataField dataField in dataFields)
            {
                string keyLower = dataField.Key.ToLowerInvariant();

                if (dataFieldsDictionary.ContainsKey(keyLower))
                {
                    continue;
                }

                DataFieldValue machineValue = dataField.DataFieldValues.FirstOrDefault(dfv => dfv.MachineId == machineId && dfv.EnvironmentId == environmentId);

                if (machineValue != null)
                {
                    dataFieldsDictionary.Add(keyLower, machineValue.Value);
                    continue;
                }

                DataFieldValue environmentValue = dataField.DataFieldValues.FirstOrDefault(dfv => dfv.EnvironmentId == environmentId );

                if (environmentValue != null)
                {
                    dataFieldsDictionary.Add(keyLower, environmentValue.Value);
                    continue;
                }
            }
            return dataFieldsDictionary;
        }

        private IDictionary<string, string> CreateEnvironmentDictionary(BundleVersion bundleVersion)
        {
            IDictionary<string, string> environmentDictionary = new Dictionary<string, string>();

            environmentDictionary.Add("version", bundleVersion.Name);
            environmentDictionary.Add("previousversion",  bundleVersion.ParentBundleVersion != null  ? bundleVersion.ParentBundleVersion.Name : "");

            environmentDictionary.Add("bundle", bundleVersion.Bundle.Name);
            return environmentDictionary;
        }
    }
}