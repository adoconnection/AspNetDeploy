using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using SatelliteService;
using SatelliteService.Bootstrapper;
using SatelliteService.Exceptions;

namespace SatelliteHost
{
    class Program
    {
        public static bool Close = false;

        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            Uri httpUrl = new Uri(ConfigurationManager.AppSettings["ServiceURI"]);

            ServiceHost host = new ServiceHost(typeof(DeploymentService), httpUrl);

            WSHttpBinding wsHttpBinding = new WSHttpBinding();
            wsHttpBinding.MaxBufferPoolSize = 1024 * 1024 * 10;
            wsHttpBinding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            wsHttpBinding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;
            

            wsHttpBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.SendTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.ReceiveTimeout = new TimeSpan(3, 0, 0);

            wsHttpBinding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            host.AddServiceEndpoint(typeof(IDeploymentService), wsHttpBinding, "");

            ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
            serviceMetadataBehavior.HttpGetEnabled = true;
            serviceMetadataBehavior.HttpGetUrl = new Uri("http://localhost:8091/AspNetDeploySatellite/Metadata");
            host.Description.Behaviors.Add(serviceMetadataBehavior);

            ServiceCredentials credentials = new ServiceCredentials();
            credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new ConfigSourceValidator();
            host.Description.Behaviors.Add(credentials);

            host.Description.Behaviors.OfType<ServiceDebugBehavior>().First().IncludeExceptionDetailInFaults = true;

            host.Credentials.ServiceCertificate.SetCertificate(
                StoreLocation.LocalMachine,
                StoreName.My,
                X509FindType.FindBySubjectName,
                "SAND.office.documentoved.ru");
            
            host.Open();
            host.Faulted += (sender, eventArgs) =>
            {
                Console.WriteLine("Error");
                Close = true;
            };
            
            Console.WriteLine("Running");

            while (!Close)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Closed");

            host.Close();
        }
    }
}
