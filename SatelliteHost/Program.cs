using System;
using System.Collections;
using System.Collections.Generic;
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

            bool isAuthorizationEnabled = bool.Parse(ConfigurationManager.AppSettings["Authorization.Enabled"]);
            bool isMetadataEnabled = bool.Parse(ConfigurationManager.AppSettings["Metadata.Enabled"]);
            string certificateName = ConfigurationManager.AppSettings["Authorization.CertificateFriendlyName"];

            Uri serviceGenericUri = new Uri(ConfigurationManager.AppSettings["Service.URI"]);

            Uri deploymentServiceUri = new Uri(ConfigurationManager.AppSettings["DeploymentService.Endpoint.URI"] ?? (serviceGenericUri + "/DeploymentService"));
            Uri deploymentServicMetadataeUri = new Uri(ConfigurationManager.AppSettings["DeploymentService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/DeploymentServiceMetadata");

            Uri monitoringServiceUri = new Uri(ConfigurationManager.AppSettings["MonitoringService.Endpoint.URI"] ?? (serviceGenericUri + "/MonitoringService"));
            Uri monitoringServiceMetadataUri = new Uri(ConfigurationManager.AppSettings["MonitoringService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/MonitoringServiceMetadata");

            Uri informationServiceUri = new Uri(ConfigurationManager.AppSettings["InformationService.Endpoint.URI"] ?? (serviceGenericUri + "/InformationService"));
            Uri informationServiceMetadataUri = new Uri(ConfigurationManager.AppSettings["InformationService.Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/InformationServiceMetadata");

            IList<ServiceHostContainer> hostContainers = new List<ServiceHostContainer>()
            {
                new ServiceHostContainer(
                    typeof(DeploymentService),
                    typeof(IDeploymentService),
                    deploymentServiceUri,
                    deploymentServicMetadataeUri,
                    isAuthorizationEnabled,
                    isMetadataEnabled,
                    certificateName),

                new ServiceHostContainer(
                    typeof(MonitoringService),
                    typeof(IMonitoringService),
                    monitoringServiceUri,
                    monitoringServiceMetadataUri,
                    isAuthorizationEnabled,
                    isMetadataEnabled,
                    certificateName),

                new ServiceHostContainer(
                    typeof(InformationService),
                    typeof(IInformationService),
                    informationServiceUri,
                    informationServiceMetadataUri,
                    isAuthorizationEnabled,
                    isMetadataEnabled,
                    certificateName),
            };

            foreach (ServiceHostContainer serviceHostContainer in hostContainers)
            {
                serviceHostContainer.StartService();
                serviceHostContainer.StartMonitoring();
            }

            Console.WriteLine("STARTED, press any key to terminate");
            Console.ReadKey();


            foreach (ServiceHostContainer serviceHostContainer in hostContainers)
            {
                serviceHostContainer.Stop();
            }

            /*
             
             netsh http add sslcert ipport=0.0.0.0:8090 certhash=111fdaf48275953db528be89fac8f0324c735297 appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}
            
             * sc create "AspNetDeploy Satellite" binpath= "D:\Services\AspNetDeploySatellite\Service\SatelliteServiceHost.exe"
             * 
             */


        }
    }
}
