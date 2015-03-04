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

            ObjectFactoryConfigurator.Configure();

            bool isAuthorizationEnabled = bool.Parse(ConfigurationManager.AppSettings["Authorization.Enabled"]);
            bool isMetadataEnabled = bool.Parse(ConfigurationManager.AppSettings["Metadata.Enabled"]);
            string certificateName = ConfigurationManager.AppSettings["Authorization.CertificateFriendlyName"];

            Uri deploymentServiceUri = new Uri(ConfigurationManager.AppSettings["DeploymentService.Endpoint.URI"]);
            Uri deploymentServicMetadataeUri = new Uri(ConfigurationManager.AppSettings["DeploymentService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/DeploymentServiceMetadata");

            Uri monitoringServiceUri = new Uri(ConfigurationManager.AppSettings["MonitoringService.Endpoint.URI"]);
            Uri monitoringServiceMetadataUri = new Uri(ConfigurationManager.AppSettings["MonitoringService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/MonitoringServiceMetadata");

            Uri informationServiceUri = new Uri(ConfigurationManager.AppSettings["InformationService.Endpoint.URI"]);
            Uri informationServiceMetadataUri = new Uri(ConfigurationManager.AppSettings["InformationService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/InformationServiceMetadata");

            ServiceHostFactory serviceHostFactory = new ServiceHostFactory();

            ServiceHost deploymentServiceHost = serviceHostFactory.Create(typeof(DeploymentService), typeof(IDeploymentService), deploymentServiceUri, isAuthorizationEnabled, isMetadataEnabled, deploymentServicMetadataeUri, certificateName);
            ServiceHost managementServiceHost = serviceHostFactory.Create(typeof(MonitoringService), typeof(IMonitoringService), monitoringServiceUri, isAuthorizationEnabled, isMetadataEnabled, monitoringServiceMetadataUri, certificateName);
            ServiceHost informationService = serviceHostFactory.Create(typeof(InformationService), typeof(IInformationService), informationServiceUri, isAuthorizationEnabled, isMetadataEnabled, informationServiceMetadataUri, certificateName);

            StartService(deploymentServiceHost);
            StartService(managementServiceHost);
            StartService(informationService);


            Console.WriteLine("STARTED, press any key to terminate");
            Console.ReadKey();

            deploymentServiceHost.Close();
            managementServiceHost.Close();
            informationService.Close();

            /*
             
             netsh http add sslcert ipport=0.0.0.0:8090 certhash=111fdaf48275953db528be89fac8f0324c735297 appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}
            
             * sc create "AspNetDeploy Satellite" binpath= "D:\Services\AspNetDeploySatellite\Service\SatelliteServiceHost.exe"
             * 
             */


        }

        private static void StartService(ServiceHost deploymentServiceHost)
        {
            deploymentServiceHost.Open();
            deploymentServiceHost.Faulted += (sender, eventArgs) =>
            {
                deploymentServiceHost.Close();
                deploymentServiceHost.Open();
            };
        }
    }
}
