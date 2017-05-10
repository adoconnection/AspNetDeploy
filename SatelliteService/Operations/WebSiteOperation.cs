using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CertificateHelpers;
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
            using (ServerManager serverManager = new ServerManager())
            {
                this.StopSite(serverManager, (string) this.configuration.siteName);

                if (Directory.Exists((string) this.configuration.destination))
                {
                    this.backupDirectoryGuid = this.BackupRepository.StoreDirectory((string) this.configuration.destination);
                    DirectoryHelper.DeleteContents((string) this.configuration.destination);
                }

                packageRepository.ExtractProject(
                    (int) this.configuration.projectId,
                    (string) this.configuration.destination);

                Site site = this.Site(serverManager, (string) this.configuration.siteName);
                //this.backupSiteConfigurationGuid = this.BackupRepository.StoreObject(site);

                ApplicationPool applicationPool = this.ApplicationPool(serverManager, (string) this.configuration.applicationPoolName);
                Application application = this.Application(serverManager, site, (string) this.configuration.destination);

                site.Applications["/"].VirtualDirectories["/"].PhysicalPath = (string) this.configuration.destination;
                site.ApplicationDefaults.ApplicationPoolName = (string) configuration.applicationPoolName;
                site.ServerAutoStart = true;
                applicationPool.AutoStart = true;

                site.Bindings.Clear();

                foreach (dynamic bindingConfig in configuration.bindings)
                {
                    Binding binding = site.Bindings.CreateElement();

                    string ip = (string) bindingConfig.IP;

                    binding.Protocol = (string) bindingConfig.protocol;
                    binding.BindingInformation = (string.IsNullOrWhiteSpace(ip) ? "" : ip) + ":" + (int) bindingConfig.port + ":" + (string) bindingConfig.host;

                    switch (binding.Protocol.ToLower())
                    {
                        case "http":
                            site.Bindings.Add(binding);
                            break;

                        default:
                        {
                            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                            store.Open(OpenFlags.OpenExistingOnly);
                            X509Certificate2 certificate = store.FindByFriendlyName((string) bindingConfig.certificateName);

                            if (certificate != null)
                            {
                                binding.CertificateHash = certificate.GetCertHash();
                                binding.CertificateStoreName = store.Name;

                                site.Bindings.Add(binding);
                            }

                            store.Close();
                        }
                            break;
                    }
                }

                serverManager.CommitChanges();
            }

            try
            {
                using (ServerManager serverManager = new ServerManager())
                {
                    this.StartSite(serverManager, (string) this.configuration.siteName);
                    serverManager.CommitChanges();
                }
            }
            catch (Exception e)
            {
                // ignore, will pop for just created sites
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
            /*else if (Directory.Exists((string) this.configuration.destination))
            {
                Directory.Delete((string) this.configuration.destination, true);
            }
*/
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

        private void StopSite(ServerManager iisManager, string siteName)
        {
            Site site = iisManager.Sites[siteName];

            if (site == null)
            {
                return;
            }

            switch (site.State)
            {
                case ObjectState.Started:
                    site.Stop();
                    break;

                case ObjectState.Starting:
                    ThreadService.SleepUntil(() => site.State == ObjectState.Started, 3);
                    site.Stop();
                    break;

                case ObjectState.Stopping:
                    break;

                case ObjectState.Stopped:
                    return;
            }

            ThreadService.SleepUntil(() => site.State == ObjectState.Stopped, 3);

            List<string> appPoolNames = site.Applications.Select( app => app.ApplicationPoolName).Distinct().ToList();

            foreach (string appPoolName in appPoolNames)
            {
                ApplicationPool applicationPool = iisManager.ApplicationPools[appPoolName];

                switch (applicationPool.State)
                {
                    case ObjectState.Started:
                        applicationPool.Stop();
                        break;

                    case ObjectState.Starting:
                        ThreadService.SleepUntil(() => applicationPool.State == ObjectState.Started, 3);
                        applicationPool.Stop();
                        break;

                    case ObjectState.Stopping:
                        break;

                    case ObjectState.Stopped:
                        return;
                }
            }

            ThreadService.SleepUntil(() => appPoolNames.All( a => iisManager.ApplicationPools[a].State == ObjectState.Stopped), 3);
        }

        private void StartSite(ServerManager iisManager, string siteName)
        {
            Site site = iisManager.Sites[siteName];

            if (site == null)
            {
                return;
            }

            switch (site.State)
            {
                case ObjectState.Started:
                    return;

                case ObjectState.Starting:
                    break;

                case ObjectState.Stopped:
                    site.Start();
                    break;

                case ObjectState.Stopping:
                    ThreadService.SleepUntil(() => site.State == ObjectState.Stopped, 3);
                    site.Start();
                    break;
            }

            ThreadService.SleepUntil(() => site.State == ObjectState.Started, 3);

            List<string> appPoolNames = site.Applications.Select(app => app.ApplicationPoolName).Distinct().ToList();

            foreach (string appPoolName in appPoolNames)
            {
                ApplicationPool applicationPool = iisManager.ApplicationPools[appPoolName];

                switch (applicationPool.State)
                {
                    case ObjectState.Started:
                        return;

                    case ObjectState.Starting:
                        break;

                    case ObjectState.Stopped:
                        applicationPool.Start();
                        break;

                    case ObjectState.Stopping:
                        ThreadService.SleepUntil(() => applicationPool.State == ObjectState.Stopped, 3);
                        applicationPool.Start();
                        break;
                }
            }

            ThreadService.SleepUntil(() => appPoolNames.All(a => iisManager.ApplicationPools[a].State == ObjectState.Started), 3);
        }
    }
}