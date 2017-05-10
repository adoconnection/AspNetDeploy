using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using SatelliteService;
using SatelliteService.Bootstrapper;

namespace SatelliteServiceHost
{
    public partial class SatelliteServiceContainer : ServiceBase
    {
        private readonly IList<ServiceHostContainer> hostContainers;

        public SatelliteServiceContainer()
        {
            InitializeComponent();

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

            this.hostContainers = new List<ServiceHostContainer>()
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
        }

        protected override void OnStart(string[] args)
        {
            foreach (ServiceHostContainer serviceHostContainer in hostContainers)
            {
                serviceHostContainer.StartService();
                serviceHostContainer.StartMonitoring();
            }

            base.OnStart(args);
        }

        protected override void OnShutdown()
        {
            foreach (ServiceHostContainer serviceHostContainer in hostContainers)
            {
                serviceHostContainer.Stop();
            }

            base.OnShutdown();
        }
    }
}
