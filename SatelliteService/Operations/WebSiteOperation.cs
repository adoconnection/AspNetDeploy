using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.Administration;
using SatelliteService.Contracts;
using SatelliteService.Helpers;

namespace SatelliteService.Operations
{
    public class WebSiteOperation : Operation
    {
        private readonly IPackageRepository packageRepository;
        private dynamic configuration;

        private Guid? backupDirectoryGuid = null;
        private Guid? backupSiteConfigurationGuid = null;

        public WebSiteOperation(IBackupRepository backupRepository, IPackageRepository packageRepository) : base(backupRepository)
        {
            this.packageRepository = packageRepository;
        }

        public void Configure(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public override void Run()
        {
            this.backupDirectoryGuid = this.BackupRepository.StoreDirectory((string)this.configuration.destination);
            
            DirectoryHelper.DeleteContents((string)this.configuration.destination);

            packageRepository.ExtractProject(
                (int)this.configuration.projectId, 
                (string)this.configuration.destination);

            using (ServerManager serverManager = new ServerManager())
            {
                Site site = this.Site(serverManager, (string) this.configuration.siteName);
                //this.backupSiteConfigurationGuid = this.BackupRepository.StoreObject(site);

                ApplicationPool applicationPool = this.ApplicationPool(serverManager, (string) this.configuration.applicationPoolName);
                Application application = this.Application(serverManager, site, (string)this.configuration.destination);

                site.Applications["/"].VirtualDirectories["/"].PhysicalPath = (string)this.configuration.destination;
                site.ApplicationDefaults.ApplicationPoolName = (string)configuration.applicationPoolName;
                site.ServerAutoStart = true;
                applicationPool.AutoStart = true;

                site.Bindings.Clear();

                foreach (dynamic bindingConfig in configuration.bindings)
                {
                    Binding binding = site.Bindings.CreateElement();

                    binding.Protocol = (string)bindingConfig.protocol;
                    binding.BindingInformation = ":" + (int)bindingConfig.port + ":" + (string)bindingConfig.url;

                    site.Bindings.Add(binding);
                }

                serverManager.CommitChanges();
            }
        }

        private Application Application(ServerManager serverManager, Site site, string physicalPath)
        {
            if (site.Applications.Count == 0)
            {
                return site.Applications.Add("/", physicalPath);
            }

            return site.Applications["/"];
        }

        private Site Site(ServerManager serverManager, string siteName)
        {
            Site site = serverManager.Sites[siteName];

            if (site == null)
            {
                site = serverManager.Sites.CreateElement();
                site.Id = serverManager.Sites.Max(s => s.Id) + 1;
                site.Name = siteName;
                serverManager.Sites.Add(site);
            }

            return site;
        }

        private ApplicationPool ApplicationPool(ServerManager serverManager, string applicationPoolName)
        {
            ApplicationPool applicationPool = serverManager.ApplicationPools[applicationPoolName];

            if (applicationPool == null)
            {
                applicationPool = serverManager.ApplicationPools.Add(applicationPoolName);
            }

            return applicationPool;
        }

        public override void Rollback()
        {
            if (this.backupDirectoryGuid.HasValue)
            {
                this.BackupRepository.RestoreDirectory(this.backupDirectoryGuid.Value);
            }

            /*if (this.backupSiteConfigurationGuid.HasValue)
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    Site site = serverManager.Sites[(string) configuration.siteName];
                    Site restoredSite = this.BackupRepository.RestoreObject<Site>(this.backupSiteConfigurationGuid.Value);

                    serverManager.Sites.Remove(site);
                    serverManager.Sites.Add(restoredSite);
                    serverManager.CommitChanges();
                }
            }*/

        }
    }
}