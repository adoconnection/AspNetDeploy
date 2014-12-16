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
        private readonly ServiceHost serviceHost;

        public SatelliteServiceContainer()
        {
            InitializeComponent();

            ObjectFactoryConfigurator.Configure();

            Uri serviceUri = new Uri(ConfigurationManager.AppSettings["Service.URI"]);
            bool isAuthorizationEnabled = bool.Parse(ConfigurationManager.AppSettings["Authrozation.Enabled"]);
            bool isMetadataEnabled = bool.Parse(ConfigurationManager.AppSettings["Metadata.Enabled"]);
            string certificateName = ConfigurationManager.AppSettings["Authrozation.CertificateFriendlyName"];
            Uri metadataUri = new Uri(ConfigurationManager.AppSettings["Metadata.Uri"] ?? "http://localhost:8091/AspNetDeploySatellite/Metadata");

            ServiceHostFactory serviceHostFactory = new ServiceHostFactory();

            serviceHost = serviceHostFactory.Create(
                serviceUri,
                isAuthorizationEnabled,
                isMetadataEnabled,
                metadataUri,
                certificateName);
        }

        protected override void OnStart(string[] args)
        {
            serviceHost.Open();
            serviceHost.Faulted += (sender, eventArgs) => this.serviceHost.Close();
        }

        protected override void OnStop()
        {
            serviceHost.Close();
        }
    }
}
