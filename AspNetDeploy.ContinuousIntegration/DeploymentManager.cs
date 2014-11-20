using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.ContinuousIntegration
{
    public class DeploymentManager
    {
        private readonly IPathServices pathServices;
        private readonly IDeploymentAgentFactory deploymentAgentFactory;

        public DeploymentManager(IPathServices pathServices, IDeploymentAgentFactory deploymentAgentFactory)
        {
            this.pathServices = pathServices;
            this.deploymentAgentFactory = deploymentAgentFactory;
        }

        public void Deploy(int packageId, int environmentId, Action<int> machineDeploymentStarted, Action<int, bool> machineDeploymentComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            Package package = entities.Package
                .Include("BundleVersion.Properties")
                .Include("BundleVersion.DeploymentSteps.Properties")
                .Include("BundleVersion.DeploymentSteps.MachineRoles")
                .First(p => p.Id == packageId);

            Environment environment = entities.Environment
                .Include("Properties")
                .Include("Machines.MachineRoles")
                .First(e => e.Id == environmentId);

            IDictionary<Machine, IDeploymentAgent> agents = this.CreateDeploymentAgents(environment, package.BundleVersion);

            if (!this.ValidateDeploymentAgents(agents))
            {
                return;
            }

            string bundlePackagePath = pathServices.GetBundlePackagePath(package.BundleVersionId, package.Id);

            if (!this.ValidatePackage(bundlePackagePath))
            {
                return;
            }

            Publication publication = this.CreatePublication(package, environment, entities);

            this.ChangePublicationResult(publication, PublicationResult.InProgress, entities);

            foreach (KeyValuePair<Machine, IDeploymentAgent> pair in agents)
            {
                Machine machine = pair.Key;
                IDeploymentAgent deploymentAgent = pair.Value;

                if (!deploymentAgent.IsReady())
                {
                    this.ChangePublicationResult(publication, PublicationResult.Error, entities);
                    return;
                }

                machineDeploymentStarted(machine.Id);

                try
                {
                    deploymentAgent.BeginPublication(publication.Id);
                    deploymentAgent.ResetPackage();
                    deploymentAgent.UploadPackage(bundlePackagePath);

                    foreach (DeploymentStep deploymentStep in this.GetMachineDeploymentSteps(package, machine))
                    {
                        deploymentAgent.ProcessDeploymentStep(deploymentStep);
                    }

                    deploymentAgent.Commit();
                }
                catch (Exception e)
                {
                    deploymentAgent.Rollback();
                    machineDeploymentComplete(machine.Id, false);
                    this.ChangePublicationResult(publication, PublicationResult.Error, entities);
                    return;
                }

                
                machineDeploymentComplete(machine.Id, true);
            }

            this.ChangePublicationResult(publication, PublicationResult.Complete, entities);
        }

       


        private IList<DeploymentStep> GetMachineDeploymentSteps(Package package, Machine machine)
        {
            return package.BundleVersion.DeploymentSteps.Where( ds => ds.MachineRoles.Intersect(machine.MachineRoles).Any()).ToList();
        }

        private bool ValidatePackage(string bundlePackagePath)
        {
            if (!File.Exists(bundlePackagePath))
            {
                return false;
            }

            return true;
        }

        private Publication CreatePublication(Package package, Environment environment, AspNetDeployEntities entities)
        {
            Publication publication = new Publication();
            publication.Package = package;
            publication.Environment = environment;
            publication.CreatedDate = DateTime.UtcNow;
            publication.Result = PublicationResult.NotStarted;

            entities.Publication.Add(publication);
            entities.SaveChanges();
            return publication;
        }

        private void ChangePublicationResult(Publication publication, PublicationResult result , AspNetDeployEntities entities)
        {
            publication.Result = result;
            entities.SaveChanges();
        }

        private bool ValidateDeploymentAgents(IDictionary<Machine, IDeploymentAgent> agents)
        {
            foreach (IDeploymentAgent agent in agents.Values)
            {
                agent.Rollback();

                if (!agent.IsReady())
                {
                    return false;
                }
            }
            return true;
        }

        private IDictionary<Machine, IDeploymentAgent> CreateDeploymentAgents(Environment environment, BundleVersion bundleVersion)
        {
            IDictionary<Machine, IDeploymentAgent> agents = new Dictionary<Machine, IDeploymentAgent>();

            foreach (Machine machine in environment.Machines)
            {
                agents[machine] = this.deploymentAgentFactory.Create(machine, bundleVersion);
            }
            return agents;
        }
    }
}