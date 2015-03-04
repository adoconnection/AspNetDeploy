using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceProcess;
using SatelliteService;
using SatelliteService.Bootstrapper;

namespace SatelliteServiceHost
{
    public partial class SatelliteServiceContainer : ServiceBase
    {
        private readonly ServiceHost deploymentServiceHost;
        private readonly ServiceHost managementServiceHost;
        private readonly ServiceHost informationService;

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

            ServiceHostFactory serviceHostFactory = new ServiceHostFactory();

            this.deploymentServiceHost = serviceHostFactory.Create(typeof(DeploymentService), typeof(IDeploymentService), deploymentServiceUri, isAuthorizationEnabled, isMetadataEnabled, deploymentServicMetadataeUri, certificateName);
            this.managementServiceHost = serviceHostFactory.Create(typeof(MonitoringService), typeof(IMonitoringService), monitoringServiceUri, isAuthorizationEnabled, isMetadataEnabled, monitoringServiceMetadataUri, certificateName);
            this.informationService = serviceHostFactory.Create(typeof(InformationService), typeof(IInformationService), informationServiceUri, isAuthorizationEnabled, isMetadataEnabled, informationServiceMetadataUri, certificateName);
        }

        protected override void OnStart(string[] args)
        {
            this.deploymentServiceHost.Open();
            this.deploymentServiceHost.Faulted += (sender, eventArgs) =>
            {
                this.deploymentServiceHost.Close();
                this.deploymentServiceHost.Open();
            };

            this.managementServiceHost.Open();
            this.managementServiceHost.Faulted += (sender, eventArgs) =>
            {
                this.managementServiceHost.Close();
                this.managementServiceHost.Open();
            };

            this.informationService.Open();
            this.informationService.Faulted += (sender, eventArgs) =>
            {
                this.informationService.Close();
                this.informationService.Open();
            };
        }

        protected override void OnStop()
        {
            this.deploymentServiceHost.Close();
            this.managementServiceHost.Close();
            this.informationService.Close();
        }
    }
}
