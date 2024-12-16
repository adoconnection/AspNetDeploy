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
        private string targetFile;

        public ConfigOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            this.targetFile = (string)configuration.File;
            string content = configuration.Content;

            if (File.Exists(targetFile))
            {
                this.backupConfigGuid = this.BackupRepository.StoreFile(targetFile);
            }

            if (this.IsContentXml(content))
            {
                XmlDocument target = new XmlDocument();

                if (File.Exists(targetFile))
                {
                    target.Load(targetFile);
                }

                XmlDocument source = new XmlDocument();
                source.LoadXml(content);

                XmlMerger xmlMerger = new XmlMerger(target, new Dictionary<string, object>());
                xmlMerger.ApplyChanges(source);

                target.Save(targetFile);
            }
            else
            {
                if (!File.Exists(targetFile))
                {
                    File.WriteAllText(targetFile, content);
                    return;
                }

                string sourceText = File.ReadAllText(targetFile);
                JObject target = JObject.Parse(string.IsNullOrWhiteSpace(sourceText) ? "{}" : sourceText);
                JObject source = JObject.Parse(content);

                JsonMergeSettings jsonMergeSettings = new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Replace
                };

                target.Merge(source, jsonMergeSettings);

                File.WriteAllText(targetFile, target.ToString());
            }
        }

        public override void Rollback()
        {
            if (this.backupConfigGuid.HasValue)
            {
                this.BackupRepository.RestoreFile(this.backupConfigGuid.Value);
            }
            else if (!string.IsNullOrWhiteSpace(this.targetFile))
            {
                File.Delete(this.targetFile);
            }
        }

        private bool IsFileXml(string path)
        {
            try
            {
                XDocument xDocument = XDocument.Load(path);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool IsContentXml(string content)
        {
            return content.TrimStart().StartsWith("<");
        }
    }
}