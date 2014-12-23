using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;

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


        public void Deploy(int publicationId, Action<int> machineDeploymentStarted, Action<int, bool> machineDeploymentComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            Publication publication = entities.Publication
                .Include("Package.BundleVersion.Properties")
                .Include("Package.BundleVersion.DeploymentSteps.Properties")
                .Include("Package.BundleVersion.DeploymentSteps.MachineRoles")
                .Include("Environment.Properties")
                .Include("Environment.Machines.MachineRoles")
                .First(p => p.Id == publicationId);

            IDictionary<Machine, IDeploymentAgent> agents = this.CreateDeploymentAgents(publication);
            IDictionary<Machine, MachinePublication> machinePublications = this.CreateMachinePublications(publication, entities);

            if (!this.ValidateDeploymentAgents(agents))
            {
                return;
            }

            string bundlePackagePath = pathServices.GetBundlePackagePath(publication.Package.BundleVersionId, publication.Package.Id);

            if (!this.ValidatePackage(bundlePackagePath))
            {
                return;
            }

            this.ChangePublicationResult(publication, PublicationState.InProgress, entities);

            foreach (MachinePublication machinePublication in machinePublications.Values)
            {
                this.ChangeMachinePublication(machinePublication, MachinePublicationState.Queued, entities);
            }

            foreach (KeyValuePair<Machine, IDeploymentAgent> pair in agents)
            {
                Machine machine = pair.Key;
                IDeploymentAgent deploymentAgent = pair.Value;

                MachinePublication machinePublication = machinePublications[machine];

                if (!deploymentAgent.IsReady())
                {
                    this.ChangePublicationResult(publication, PublicationState.Error, entities);
                    return;
                }

                machineDeploymentStarted(machine.Id);

                try
                {
                    deploymentAgent.BeginPublication(publication.Id);
                    deploymentAgent.ResetPackage();

                    this.ChangeMachinePublication(machinePublication, MachinePublicationState.Uploading, entities);
                    deploymentAgent.UploadPackage(bundlePackagePath);

                    this.ChangeMachinePublication(machinePublication, MachinePublicationState.Configuring, entities);

                    IList<DeploymentStep> machineDeploymentSteps = this.GetMachineDeploymentSteps(publication.Package, machine);

                    foreach (DeploymentStep deploymentStep in machineDeploymentSteps)
                    {
                        try
                        {
                            this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepConfiguring);
                            deploymentAgent.ProcessDeploymentStep(deploymentStep);
                            this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepConfiguringComplete);
                        }
                        catch (Exception e)
                        {
                            this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepConfiguringError, this.GetLastExceptionSafe(deploymentAgent));
                            throw;
                        }
                    }

                    this.ChangeMachinePublication(machinePublication, MachinePublicationState.Running, entities);

                    foreach (DeploymentStep deploymentStep in machineDeploymentSteps)
                    {
                        try
                        {
                            this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepExecuting);
                            
                            if (deploymentAgent.ExecuteNextOperation())
                            {
                                this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepExecutingComplete);    
                            }
                            else
                            {
                                throw new AspNetDeployException("Error executing deploymentStep");
                            }
                        }
                        catch (Exception e)
                        {
                            this.LogMachinePublicationStep(machinePublication, deploymentStep, entities, MachinePublicationLogEvent.DeploymentStepExecutingError, this.GetLastExceptionSafe(deploymentAgent));
                            throw;
                        }
                    }

                    deploymentAgent.Complete();
                }
                catch (Exception e)
                {
                    /*deploymentAgent.Rollback();*/
                    machineDeploymentComplete(machine.Id, false);
                    this.ChangeMachinePublication(machinePublication, MachinePublicationState.Error, entities);
                    this.ChangePublicationResult(publication, PublicationState.Error, entities);
                    return;
                }

                this.ChangeMachinePublication(machinePublication, MachinePublicationState.Complete, entities);
                machineDeploymentComplete(machine.Id, true);
            }

            this.ChangePublicationResult(publication, PublicationState.Complete, entities);
        }

        private IExceptionInfo GetLastExceptionSafe(IDeploymentAgent deploymentAgent)
        {
            try
            {
                return deploymentAgent.GetLastException();
            }
            catch
            {
                return null;
            }
        }

        private void LogMachinePublicationStep(MachinePublication machinePublication, DeploymentStep deploymentStep, AspNetDeployEntities entities, MachinePublicationLogEvent @event, IExceptionInfo lastException = null)
        {
            MachinePublicationLog machinePublicationLog = new MachinePublicationLog();
            machinePublicationLog.CreatedDate = DateTime.UtcNow;
            machinePublicationLog.MachinePublication = machinePublication;
            machinePublicationLog.Event = @event;
            machinePublicationLog.DeploymentStepId = deploymentStep.Id;
            entities.MachinePublicationLog.Add(machinePublicationLog);

            if (lastException != null)
            {
                this.RecordException(entities, null, lastException, machinePublicationLog);
            }
            
            entities.SaveChanges();
        }

        private void RecordException(AspNetDeployEntities entities, ExceptionEntry parentException, IExceptionInfo lastException, MachinePublicationLog machinePublicationLog)
        {
            ExceptionEntry exceptionEntry = new ExceptionEntry();
            machinePublicationLog.Exception = exceptionEntry;
            exceptionEntry.Message = lastException.Message;
            exceptionEntry.Source = lastException.Source;
            exceptionEntry.StackTrace = lastException.StackTrace;
            exceptionEntry.TypeName = lastException.TypeName;
            entities.ExceptionEntry.Add(exceptionEntry);

            if (parentException != null)
            {
                parentException.InnerExceptionEntry = exceptionEntry;
            }

            foreach (IExceptionDataInfo exceptionDataInfo in lastException.ExceptionData)
            {
                ExceptionEntryData data = new ExceptionEntryData();
                data.ExceptionEntry = exceptionEntry;
                data.IsProperty = exceptionDataInfo.IsProperty;
                data.Name = exceptionDataInfo.Name;
                data.Value = exceptionDataInfo.Value;
                entities.ExceptionEntryData.Add(data);
            }

            if (lastException.InnerException != null)
            {
                this.RecordException(entities, exceptionEntry, lastException.InnerException, machinePublicationLog);
            }
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

        private void ChangePublicationResult(Publication publication, PublicationState result , AspNetDeployEntities entities)
        {
            publication.State = result;
            entities.SaveChanges();
        }

        private void ChangeMachinePublication(MachinePublication machinePublication, MachinePublicationState result, AspNetDeployEntities entities)
        {
            machinePublication.State = result;
            entities.SaveChanges();
        }

        private bool ValidateDeploymentAgents(IDictionary<Machine, IDeploymentAgent> agents)
        {
            foreach (IDeploymentAgent agent in agents.Values)
            {
                try
                {
                    agent.Rollback();
                    return agent.IsReady();
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
            return true;
        }

        private IDictionary<Machine, IDeploymentAgent> CreateDeploymentAgents(Publication publication)
        {
            IDictionary<Machine, IDeploymentAgent> agents = new Dictionary<Machine, IDeploymentAgent>();

            foreach (Machine machine in publication.Environment.Machines)
            {
                agents[machine] = this.deploymentAgentFactory.Create(machine, publication.Package.BundleVersion);
            }

            return agents;
        }

        private IDictionary<Machine, MachinePublication> CreateMachinePublications(Publication publication, AspNetDeployEntities entities)
        {
            IDictionary<Machine, MachinePublication> machinePublications = new Dictionary<Machine, MachinePublication>();

            foreach (Machine machine in publication.Environment.Machines)
            {
                MachinePublication machinePublication = entities.MachinePublication.FirstOrDefault(mp => mp.MachineId == machine.Id && mp.PublicationId == publication.Id);

                if (machinePublication == null)
                {
                    machinePublication = new MachinePublication();

                    machinePublication.Publication = publication;
                    machinePublication.Machine = machine;
                    machinePublication.CreatedDate = DateTime.UtcNow;

                    entities.MachinePublication.Add(machinePublication);
                    entities.SaveChanges();
                }

                machinePublications.Add(machine, machinePublication);
            }

            return machinePublications;
        }
    }
}