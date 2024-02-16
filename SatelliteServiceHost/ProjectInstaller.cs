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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.Win32;
using SatelliteService;

namespace SatelliteServiceHost
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private string uninstallUri;
        private string uninstallBackupsPath;
        private string uninstallPackagesPath;

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            string exePath = this.Context.Parameters["assemblypath"];
            string directoryName = Path.GetDirectoryName(exePath);

            string login = this.Context.Parameters["login"];
            string secret = this.Context.Parameters["secret"];
            Uri uri = new Uri(this.Context.Parameters["uri"]);

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


            X509Certificate2 certificate = FindCertificate(uri.Host);

            if (certificate == null)
            {
                throw new SatelliteServiceException("Certificate not found for domain " + uri.Host);
            }

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);

            UpdateAppSettings(configuration, "PackagesPath", packagesPath);
            UpdateAppSettings(configuration, "BackupsPath", backupsPath);

            UpdateAppSettings(configuration, "Metadata.Enabled", "false");
            UpdateAppSettings(configuration, "Authorization.Enabled", "true");
            
            UpdateAppSettings(configuration, "Authorization.UserName", login);
            
            UpdateAppSettings(configuration, "Authorization.Password", secret);
            
            UpdateAppSettings(configuration, "Authorization.CertificateFriendlyName", certificate.FriendlyName);
            UpdateAppSettings(configuration, "LocalBackups.CompressionLevel", "BestCompression");
            UpdateAppSettings(configuration, "Service.URI", "https://" + uri.Host + ":" + uri.Port + "/AspNetDeploySatellite");

            UpdateAppSettings(configuration, "LocalBackups.AutoCleanupDays", "10");
            UpdateAppSettings(configuration, "Packages.AutoCleanupDays", "10");

            configuration.Save(ConfigurationSaveMode.Modified);

            Process process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "http delete sslcert ipport=0.0.0.0:" + uri.Port,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            process.WaitForExit();

            process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "http add sslcert ipport=0.0.0.0:" + uri.Port + " certhash=" + certificate.Thumbprint + " appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}",
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            process.WaitForExit();

            process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "advfirewall firewall add rule name= \"AspNetDeploy\" dir=in action=allow protocol=TCP localport=" + uri.Port,
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

        public void serviceInstaller1_BeforeInstall(object sender, InstallEventArgs e)
        {
            string uriText = this.Context.Parameters["uri"];

            if (string.IsNullOrWhiteSpace(uriText))
            {
                throw new SatelliteServiceException("URI not set");
            }

            Uri uri;

            if (!Uri.TryCreate(uriText, UriKind.Absolute, out uri))
            {
                throw new SatelliteServiceException("Invalid URI");
            }

            X509Certificate2 certificate = this.FindCertificate(uri.Host);

            if (certificate == null)
            {
                throw new SatelliteServiceException("No certificate for host: " + uri.Host);
            }
        }

        private void serviceInstaller1_BeforeUninstall(object sender, InstallEventArgs e)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(this.Context.Parameters["assemblypath"]);

            this.uninstallPackagesPath = configuration.AppSettings.Settings["PackagesPath"].Value;
            this.uninstallBackupsPath = configuration.AppSettings.Settings["BackupsPath"].Value;
            this.uninstallUri = configuration.AppSettings.Settings["Service.URI"].Value;
        }

        public void serviceInstaller1_AfterUninstall(object sender, InstallEventArgs e)
        {
            string exePath = this.Context.Parameters["assemblypath"];

            Uri uri = new Uri(this.uninstallUri);

            if (Directory.Exists(this.uninstallPackagesPath))
            {
                Directory.Delete(this.uninstallPackagesPath, true);
            }

            if (Directory.Exists(this.uninstallBackupsPath))
            {
                Directory.Delete(this.uninstallBackupsPath, true);
            }

            Process process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "http delete sslcert ipport=0.0.0.0:" + uri.Port,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            process.WaitForExit();
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

        private X509Certificate2 FindCertificate(string hostName)
        {
            X509Certificate2 findCertificate;

            if (X509Certificate2(hostName, out findCertificate, new X509Store(StoreName.My, StoreLocation.LocalMachine)))
            {
                return findCertificate;
            }

            if (X509Certificate2(hostName, out findCertificate, new X509Store(StoreName.My, StoreLocation.CurrentUser)))
            {
                return findCertificate;
            }

            if (X509Certificate2(hostName, out findCertificate, new X509Store(StoreName.Root, StoreLocation.LocalMachine)))
            {
                return findCertificate;
            }

            if (X509Certificate2(hostName, out findCertificate, new X509Store(StoreName.Root, StoreLocation.CurrentUser)))
            {
                return findCertificate;
            }

            return null;
        }

        private static bool X509Certificate2(string hostName, out X509Certificate2 outCertificate, X509Store x509Store)
        {
            x509Store.Open(OpenFlags.OpenExistingOnly);

            foreach (X509Certificate2 certificate in x509Store.Certificates.Cast<X509Certificate2>().OrderByDescending( s => s.NotAfter))
            {
                if (certificate.NotBefore > DateTime.Now || certificate.NotAfter < DateTime.Now)
                {
                    continue;
                }

                Regex regex = new Regex(@"CN=(?<val>[^;,\s\n\r]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                if (!string.IsNullOrWhiteSpace(certificate.Subject))
                {
                    Match match = regex.Match(certificate.Subject);

                    string value = match.Groups["val"].Value;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        value = value.TrimStart('*');

                        if (match.Success && hostName.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("MATCH");
                            outCertificate = certificate;
                            return true;
                        }
                    }
                }

                foreach (X509Extension extension in certificate.Extensions)
                {
                    if (extension.Oid.Value != "2.5.29.17")
                    {
                        continue;
                    }

                    foreach (string line in extension.Format(true).Split('\n'))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string normalizedLine = line.Trim();

                        if (!normalizedLine.StartsWith("DNS Name=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        string host = normalizedLine.Substring(9);
                        host = host.TrimStart('*');

                        if (hostName.EndsWith(host, StringComparison.InvariantCultureIgnoreCase))
                        {
                            {
                                outCertificate = certificate;
                                return true;
                            }
                        }
                    }
                }
            }

            x509Store.Close();
            outCertificate = null;

            return false;
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller1_AfterRollback(object sender, InstallEventArgs e)
        {

        }
    }
}
