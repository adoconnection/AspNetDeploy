using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using CertificateHelpers;

namespace SatelliteService
{
    public class ServiceHostFactory
    {
        public ServiceHost Create(Type serviceImplementationType, Type serviceInterfaceType, Uri serviceUri, bool isAuthorizationEnabled, bool isMetadataEnabled, Uri metadataUri, string certificateFriendlyName)
        {
            ServiceHost serviceHost = new ServiceHost(serviceImplementationType, serviceUri);

            WSHttpBinding wsHttpBinding = new WSHttpBinding();
            wsHttpBinding.MaxBufferPoolSize = 1024 * 1024 * 10;
            wsHttpBinding.MaxReceivedMessageSize = 1024 * 1024 * 10;
            wsHttpBinding.ReaderQuotas.MaxArrayLength = 1024 * 1024 * 10;

            wsHttpBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.CloseTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.SendTimeout = new TimeSpan(0, 10, 0);
            wsHttpBinding.ReceiveTimeout = new TimeSpan(3, 0, 0);

            wsHttpBinding.Security.Mode = isAuthorizationEnabled ? SecurityMode.TransportWithMessageCredential : SecurityMode.None;
            wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            serviceHost.AddServiceEndpoint(serviceInterfaceType, wsHttpBinding, "");

            if (isMetadataEnabled)
            {
                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.HttpGetUrl = metadataUri;
                serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            ServiceCredentials credentials = new ServiceCredentials();
            credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new ConfigSourceValidator();
            serviceHost.Description.Behaviors.Add(credentials);

            serviceHost.Description.Behaviors.OfType<ServiceDebugBehavior>().First().IncludeExceptionDetailInFaults = true;

            if (isAuthorizationEnabled)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly);

                X509Certificate2 certificate = store.FindByFriendlyName(certificateFriendlyName);

                if (certificate == null)
                {
                    throw new SatelliteServiceException("Certificate not found: " + certificateFriendlyName);
                }

                serviceHost.Credentials.ServiceCertificate.Certificate = certificate;

                store.Close();
            }

            return serviceHost;
        }
    }
}