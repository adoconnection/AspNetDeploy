using System;
using System.ServiceModel;
using System.Threading;
using SatelliteService;

namespace SatelliteServiceHost
{
    public class ServiceHostContainer
    {
        public ServiceHost ServiceHost { get; private set; }

        private readonly Type serviceType;
        private readonly Type interfaceType;

        private readonly Uri endpointUri;
        private readonly Uri metadataEndpointUri;

        private readonly bool isAuthorizationEnabled;
        private readonly bool isMetadataEnabled;
        private readonly string certificateName;

        private Thread monithringThread;

        public ServiceHostContainer(Type serviceType, Type interfaceType, Uri endpointUri, Uri metadataEndpointUri, bool isAuthorizationEnabled, bool isMetadataEnabled, string certificateName)
        {
            this.serviceType = serviceType;
            this.interfaceType = interfaceType;
            this.endpointUri = endpointUri;
            this.metadataEndpointUri = metadataEndpointUri;
            this.isAuthorizationEnabled = isAuthorizationEnabled;
            this.isMetadataEnabled = isMetadataEnabled;
            this.certificateName = certificateName;
        }

        public void StartService()
        {
            ServiceHostFactory serviceHostFactory = new ServiceHostFactory();
            this.ServiceHost = serviceHostFactory.Create(this.serviceType, this.interfaceType, this.endpointUri, this.isAuthorizationEnabled, this.isMetadataEnabled, this.metadataEndpointUri, this.certificateName);
            this.ServiceHost.Open();
        }

        public void StartMonitoring()
        {
            this.monithringThread = new Thread(this.Watcher());
        }

        public void Stop()
        {
            this.monithringThread.Abort();
            this.ServiceHost.Close();
        }

        private ThreadStart Watcher()
        {
            return () =>
            {
                try
                {
                    while (true)
                    {
                        if (this.ServiceHost == null)
                        {
                            continue;
                        }

                        if (this.ServiceHost.State == CommunicationState.Faulted || this.ServiceHost.State == CommunicationState.Closed)
                        {
                            Console.WriteLine("sdccsdc");
                            this.StartService();
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (ThreadAbortException)
                {
                    
                }
            };
        }
    }
}