using System;
using System.Globalization;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner.Jobs
{
    public class DeploymentJob
    {
        public void Start(int packageId, int environmentId, Action<int> machineDeploymentStarted, Action<int, bool> machineDeploymentComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            Package package = entities.Package
                .Include("Publications")
                .Include("BundleVersion.Properties")
                .First(p => p.Id == packageId);

            DeploymentManager deploymentManager = Factory.GetInstance<DeploymentManager>();

            deploymentManager.Deploy(
                packageId, 
                environmentId,
                machineId =>
                {
                    machineDeploymentStarted(machineId);
                },
                (machineId, isSuccess) =>
                {
                    machineDeploymentComplete(machineId, isSuccess);
                });

            entities.SaveChanges();
        }
    }
}