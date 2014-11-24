using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace SatelliteService.Exceptions
{
    public class ExceptionLoggingServiceBehavior : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                foreach (OperationDescription operation in endpoint.Contract.Operations)
                {
                    if (operation.Behaviors.Find<ExceptionLoggingOperationBehavior>() == null)
                    {
                        IOperationBehavior behavior = new ExceptionLoggingOperationBehavior();
                        operation.Behaviors.Add(behavior);
                    }
                }
            }
        }
    }
}