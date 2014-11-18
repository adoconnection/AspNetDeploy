using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
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

        public void Configure(dynamic configuration, IDictionary<string, object> variables)
        {
            this.configuration = configuration;
            base.Configure(variables);
        }

        public override void Run()
        {
            this.backupConfigGuid = this.BackupRepository.StoreFile((string)configuration.file);

            XmlDocument target = new XmlDocument();
            target.Load((string)configuration.file);

            XmlDocument source = new XmlDocument();
            source.LoadXml((string)configuration.content);

            XmlMerger xmlMerger = new XmlMerger(target, this.Variables);
            xmlMerger.ApplyChanges(source);

            target.Save((string)configuration.file);
        }

        public override void Rollback()
        {
            if (this.backupConfigGuid.HasValue)
            {
                this.BackupRepository.RestoreFile(this.backupConfigGuid.Value);
            }
        }
    }
}