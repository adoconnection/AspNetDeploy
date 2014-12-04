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
            bool authorizationEnabled = bool.Parse(ConfigurationManager.AppSettings["Authrozation.Enabled"]);
            bool metadataEnabled = bool.Parse(ConfigurationManager.AppSettings["Metadata.Enabled"]);

            string metadataURL = "http://localhost:8091/AspNetDeploySatellite/Metadata";


            ServiceHost host = new ServiceHost(typeof(DeploymentService), httpUrl);


            WSHttpBinding wsHttpBinding = new WSHttpBinding();
            wsHttpBinding.MaxBufferPoolSize = 1024 * 1024 * 10;
            wsHttpBinding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            wsHttpBinding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;
            
            wsHttpBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.SendTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.ReceiveTimeout = new TimeSpan(3, 0, 0);

            wsHttpBinding.Security.Mode = authorizationEnabled ? SecurityMode.TransportWithMessageCredential : SecurityMode.None;
            wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            host.AddServiceEndpoint(typeof(IDeploymentService), wsHttpBinding, "");

            if (metadataEnabled)
            {
                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.HttpGetUrl = new Uri(metadataURL);
                host.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            ServiceCredentials credentials = new ServiceCredentials();
            credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new ConfigSourceValidator();
            host.Description.Behaviors.Add(credentials);

            host.Description.Behaviors.OfType<ServiceDebugBehavior>().First().IncludeExceptionDetailInFaults = true;

            if (authorizationEnabled)
            {
                host.Credentials.ServiceCertificate.SetCertificate(
                    StoreLocation.LocalMachine,
                    StoreName.My,
                    X509FindType.FindBySubjectName,
                    ConfigurationManager.AppSettings["Authrozation.CertificateName"]);
            }

            host.Open();
            host.Faulted += (sender, eventArgs) =>
            {
                Console.WriteLine("Error");
                Close = true;
            };

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Running");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Version: " + (new DeploymentService()).GetVersion());
            Console.WriteLine("URL: " + ConfigurationManager.AppSettings["ServiceURI"]);
            
            if (authorizationEnabled)
            {
                Console.WriteLine("CertificateName: " + ConfigurationManager.AppSettings["Authrozation.CertificateName"]);
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

            if (metadataEnabled)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Metadata Enabled: TRUE");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Metadata URL: " + metadataURL);
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

            host.Close();

            /*
             
             netsh http add sslcert ipport=0.0.0.0:8090 certhash=969e746fb374e0dd102b0a3c197c7b5d66b0900e appid={2f244ac1-9d8d-45d8-b46b-8ba79a326ebc}
             
             */
        }
    }
}
