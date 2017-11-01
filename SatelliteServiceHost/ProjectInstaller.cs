using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading.Tasks;
using SatelliteService;

namespace SatelliteServiceHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            string exePath = this.Context.Parameters["assemblypath"];
            string directoryName = Path.GetDirectoryName(exePath);

            string certName = this.Context.Parameters["certname"];

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);

            string packagesPath = Path.Combine(directoryName, "Packages");
            string backupsPath = Path.Combine(directoryName, "Backups");

            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
            }

            if (!Directory.Exists(backupsPath))
            {
                Directory.CreateDirectory(backupsPath);
            }

            UpdateAppSettings(configuration, "PackagesPath", packagesPath);
            UpdateAppSettings(configuration, "BackupsPath", backupsPath);

            UpdateAppSettings(configuration, "Metadata.Enabled", "false");
            UpdateAppSettings(configuration, "Authorization.Enabled", "true");
            UpdateAppSettings(configuration, "Authorization.UserName", this.Context.Parameters["login"]);
            UpdateAppSettings(configuration, "Authorization.Password", this.Context.Parameters["secret"]);
            
            UpdateAppSettings(configuration, "Authorization.CertificateFriendlyName", certName);
            UpdateAppSettings(configuration, "LocalBackups.CompressionLevel", "BestCompression");
            UpdateAppSettings(configuration, "Service.URI", "https://" + this.Context.Parameters["domain"] + ":" + this.Context.Parameters["port"] + "/AspNetDeploySatellite");

            configuration.Save(ConfigurationSaveMode.Modified);

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly);

            X509Certificate2 certificate = FindByFriendlyName(store, certName);

            if (certificate == null)
            {
                throw new SatelliteServiceException("Certificate not found: " + certName);
            }

            store.Close();

            Process process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "http delete sslcert ipport=0.0.0.0:" + this.Context.Parameters["port"],
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            process.WaitForExit();

            process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "http add sslcert ipport=0.0.0.0:" + this.Context.Parameters["port"] + " certhash=" + certificate.Thumbprint + " appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}",
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            process.WaitForExit();

            try
            {
                ServiceController serviceController = new ServiceController(this.serviceInstaller1.ServiceName);
                serviceController.Start();
            }
            catch (Exception exception)
            {
            }
           
        }

        private void UpdateAppSettings(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }
        }

        private static X509Certificate2 FindByFriendlyName(X509Store store, string friendlyName)
        {
            return store.Certificates
                    .Cast<X509Certificate2>()
                    .FirstOrDefault(certificate => certificate.FriendlyName.ToLowerInvariant() == friendlyName.ToLowerInvariant());
        }
        
    }
}
