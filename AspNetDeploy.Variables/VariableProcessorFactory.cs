﻿using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.Variables
{
    public class VariableProcessorFactory : IVariableProcessorFactory
    {
        public IVariableProcessor Create(int packageId, int machineId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            List<DataField> globalDataFields = entities.DataField
                .Include("DataFieldValues.Environment")
                .Include("DataFieldValues.Machine")
                .Where( df => !df.IsDeleted && df.Mode == DataFieldMode.Global)
                .ToList();

            Package package = entities.Package
                .Include("BundleVersion")
                .First(p => p.Id == packageId);

            BundleVersion bundleVersion = entities.BundleVersion
                .Include("Bundle")
                .Include("ParentBundleVersion")
                .Include("DataFields.DataFieldValues.Environment")
                .Include("DataFields.DataFieldValues.Machine")
                .First(bv => bv.Id == package.BundleVersionId);

            List<DataField> bundleDataFields = bundleVersion.DataFields
                .Where( df => !df.IsDeleted)
                .ToList();

            int environmentId = entities.Machine.First(m => m.Id == machineId).Environments.First().Id;

            IDictionary<string, string> dataFieldsDictionary = this.CreateDataFieldsDictionary(machineId, globalDataFields, bundleDataFields, environmentId);
            IDictionary<string, string> environmentDictionary = this.CreateEnvironmentDictionary(package);

            return new VariableProcessor(dataFieldsDictionary, environmentDictionary, new Dictionary<string, Func<string, string>>()
            {
                { "domainSafe".ToLower(), s => s.Replace(".", "-") },
                { "escapePath".ToLower(), s => s.Replace("\\", "\\\\") },
            });
        }

        private IDictionary<string, string> CreateDataFieldsDictionary(int machineId, IEnumerable<DataField> globalDataFields, IEnumerable<DataField> bundleDataFields, int environmentId)
        {
            IDictionary<string, string> dataFieldsDictionary = new Dictionary<string, string>();

            foreach (DataField bundleDataField in bundleDataFields)
            {
                string keyLower = bundleDataField.Key.ToLowerInvariant();

                if (dataFieldsDictionary.ContainsKey(keyLower))
                {
                    continue;
                }

                DataFieldValue dataFieldValue = bundleDataField.DataFieldValues.FirstOrDefault();

                if (dataFieldValue != null)
                {
                    dataFieldsDictionary.Add(keyLower, dataFieldValue.Value);
                }
            }

            foreach (DataField globalDataField in globalDataFields)
            {
                string keyLower = globalDataField.Key.ToLowerInvariant();

                if (dataFieldsDictionary.ContainsKey(keyLower))
                {
                    continue;
                }

                DataFieldValue machineValue = globalDataField.DataFieldValues.FirstOrDefault(dfv => dfv.MachineId == machineId && dfv.EnvironmentId == environmentId);

                if (machineValue != null)
                {
                    dataFieldsDictionary.Add(keyLower, machineValue.Value);
                    continue;
                }

                DataFieldValue environmentValue = globalDataField.DataFieldValues.FirstOrDefault(dfv => dfv.EnvironmentId == environmentId );

                if (environmentValue != null)
                {
                    dataFieldsDictionary.Add(keyLower, environmentValue.Value);
                    continue;
                }
            }
            return dataFieldsDictionary;
        }

        private IDictionary<string, string> CreateEnvironmentDictionary(Package package)
        {


            IDictionary<string, string> environmentDictionary = new Dictionary<string, string>
            {
                { "version", package.BundleVersion.Name },
                { "date.ticks", DateTime.Now.Ticks.ToString() },
                { "date.day", DateTime.Now.Day.ToString("00") },
                { "date.month", DateTime.Now.Month.ToString("00") },
                { "date.year", DateTime.Now.Year.ToString() },
                { "package.id", package.Id.ToString() },
                { "package.day", package.CreatedDate.Day.ToString() },
                { "package.month", package.CreatedDate.Month.ToString() },
                { "package.year", package.CreatedDate.Year.ToString() },
                { "version.previous", package.BundleVersion.ParentBundleVersion != null ? package.BundleVersion.ParentBundleVersion.Name : "" },
                { "previousversion", package.BundleVersion.ParentBundleVersion != null ? package.BundleVersion.ParentBundleVersion.Name : "" },
                { "bundle", package.BundleVersion.Bundle.Name }
            };

            return environmentDictionary;
        }
    }
}