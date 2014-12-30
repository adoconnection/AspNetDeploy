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
        public void Start(int publicationId, Action<int> machineDeploymentStarted, Action<int, bool> machineDeploymentComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            Publication publication = entities.Publication
                .Include("Package.BundleVersion.Properties")
                .Include("Environment")
                .First( p => p.Id == publicationId);

            DeploymentManager deploymentManager = Factory.GetInstance<DeploymentManager>();

            DateTime deploymentStart = DateTime.UtcNow;

            deploymentManager.Deploy(
                publication.Id,
                machineId =>
                {
                    machineDeploymentStarted(machineId);
                },
                (machineId, isSuccess) =>
                {
                    machineDeploymentComplete(machineId, isSuccess);
                });

            publication.Package.BundleVersion.SetStringProperty("LastPublicationAttemptPackage", publication.PackageId.ToString());
            publication.Package.BundleVersion.SetStringProperty("LastDeploymentDuration-e" + publication.EnvironmentId, (DateTime.UtcNow - deploymentStart).TotalSeconds.ToString(CultureInfo.InvariantCulture));

            entities.SaveChanges();
        }
    }
}