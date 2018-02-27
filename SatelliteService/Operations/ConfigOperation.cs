using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SatelliteService.Contracts;
using SatelliteService.Helpers;

namespace SatelliteService.Operations
{
    public class ConfigOperation : Operation
    {
        private dynamic configuration;
        private Guid? backupConfigGuid = null;

        public ConfigOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            this.backupConfigGuid = this.BackupRepository.StoreFile((string)configuration.file);

            if (this.IsXml((string) configuration.file))
            {
                XmlDocument target = new XmlDocument();
                target.Load((string)configuration.file);

                XmlDocument source = new XmlDocument();
                source.LoadXml((string)configuration.content);

                XmlMerger xmlMerger = new XmlMerger(target, new Dictionary<string, object>());
                xmlMerger.ApplyChanges(source);

                target.Save((string)configuration.file);
            }
            else
            {
                JObject target = JObject.Parse(File.ReadAllText((string)this.configuration.file));
                JObject source = JObject.Parse((string)configuration.content);

                JsonMergeSettings jsonMergeSettings = new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace
                };

                target.Merge(source, jsonMergeSettings);

                File.WriteAllText((string)configuration.file, target.ToString());
            }
        }

        public override void Rollback()
        {
            if (this.backupConfigGuid.HasValue)
            {
                this.BackupRepository.RestoreFile(this.backupConfigGuid.Value);
            }
        }

        private bool IsXml(string text)
        {
            try
            {
                XDocument xDocument = XDocument.Load(text);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}