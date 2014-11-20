using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using SatelliteService;
using SatelliteService.Bootstrapper;

namespace SatelliteHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            Uri httpUrl = new Uri(ConfigurationManager.AppSettings["ServiceURI"]);

            ServiceHost host = new ServiceHost(typeof(DeploymentService), httpUrl);
           
            WSHttpBinding wsHttpBinding = new WSHttpBinding(SecurityMode.Message);
            wsHttpBinding.MaxBufferPoolSize = 1024 * 1024 * 10;
            wsHttpBinding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            wsHttpBinding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;

            wsHttpBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.SendTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.ReceiveTimeout = new TimeSpan(3, 0, 0);

            host.AddServiceEndpoint(typeof(IDeploymentService), wsHttpBinding, "");

            ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
            serviceMetadataBehavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(serviceMetadataBehavior);
            
            host.Open();
            
            Console.WriteLine("Running");
            Console.ReadKey();
            
            host.Close();
        }
    }
}
