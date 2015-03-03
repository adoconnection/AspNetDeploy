using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using SatelliteService;
using SatelliteService.Bootstrapper;

namespace SatelliteConsoleHost
{
    class Program
    {
        public static bool Close = false;

        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            Uri serviceUri = new Uri(ConfigurationManager.AppSettings["Service.URI"]);
            bool isAuthorizationEnabled = bool.Parse(ConfigurationManager.AppSettings["Authorization.Enabled"]);
            string certificateName = ConfigurationManager.AppSettings["Authorization.CertificateFriendlyName"];
            bool isMetadataEnabled = bool.Parse(ConfigurationManager.AppSettings["Metadata.Enabled"]);
            Uri metadataUri = new Uri(ConfigurationManager.AppSettings["Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/Metadata");

            ServiceHostFactory serviceHostFactory = new ServiceHostFactory();
            
            ServiceHost serviceHost = serviceHostFactory.Create(
                serviceUri, 
                isAuthorizationEnabled, 
                isMetadataEnabled, 
                metadataUri, 
                certificateName);

            serviceHost.Open();
            serviceHost.Faulted += (sender, eventArgs) =>
            {
                Console.WriteLine("Error");
                Close = true;
            };

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Running");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Version: " + (new DeploymentService()).GetVersion());
            Console.WriteLine("URL: " + serviceUri);
            
            if (isAuthorizationEnabled)
            {
                Console.WriteLine("CertificateName: " + certificateName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Authrozation Enabled: True");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Authrozation Enabled: FALSE");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            if (isMetadataEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Metadata Enabled: TRUE");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Metadata URL: " + metadataUri);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Metadata Enabled: False");
                Console.ForegroundColor = ConsoleColor.Gray;
            }


            while (!Close)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Closed");

            serviceHost.Close();

            /*
             
             netsh http add sslcert ipport=0.0.0.0:8090 certhash=111fdaf48275953db528be89fac8f0324c735297 appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}
            
             * sc create "AspNetDeploy Satellite" binpath= "D:\Services\AspNetDeploySatellite\Service\SatelliteServiceHost.exe"
             * 
             */


        }
    }
}
