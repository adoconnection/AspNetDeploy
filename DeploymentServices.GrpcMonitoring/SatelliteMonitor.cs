using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.Model;
using DeploymentServices.Grpc;

namespace DeploymentServices.GrpcMonitoring
{
    public class SatelliteMonitor : ISatelliteMonitor
    {
        private readonly IPathServices _pathServices;

        public SatelliteMonitor(IPathServices pathServices)
        {
            this._pathServices = pathServices;
        }

        public IServerSummary GetServerSummary(Machine machine)
        {
            if (string.IsNullOrWhiteSpace(machine.URL))
            {
                return null;
            }

            GrpcDeploymentAgent agent = new GrpcDeploymentAgent(null, this._pathServices, machine.URL, machine.Login, machine.Password);

            try
            {
                agent.IsReady();
                return null;
            }
            catch (Exception e)
            {
                if (e is MessageSecurityException)
                {
                    return null;
                }

                if (e is TimeoutException)
                {
                    return null;
                }

                if (e is EndpointNotFoundException)
                {
                    return null;
                }

                return null;
            }
        }

        public SatelliteState IsAlive(Machine machine)
        {
            if (string.IsNullOrWhiteSpace(machine.URL))
            {
                return SatelliteState.NotConfigured;
            }

            GrpcDeploymentAgent agent = new GrpcDeploymentAgent(null, this._pathServices, machine.URL, machine.Login, machine.Password);

            try
            {
                agent.IsReady();
                return SatelliteState.Alive;
            }
            catch (Exception e)
            {
                if (e is MessageSecurityException)
                {
                    return SatelliteState.UnableToEstablishSecureConnection;
                }

                if (e is TimeoutException)
                {
                    return SatelliteState.Timeout;
                }

                if (e is EndpointNotFoundException)
                {
                    return SatelliteState.Inactive;
                }

                return SatelliteState.Inactive;
            }
        }
    }
}
