using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using Microsoft.VisualBasic;
using ObjectFactory;
using ThreadHostedTaskRunner.Jobs;
using Environment = AspNetDeploy.Model.Environment;

namespace ThreadHostedTaskRunner
{
    public class ThreadTaskRunner : ITaskRunner
    {
        private readonly Timer Timer = new Timer(TimerCallback);
        private static bool IsTimerRunning = false;

        public void Initialize()
        {
            Timer.Change(new TimeSpan(0, 0, 30), new TimeSpan(0, 0, 10));
        }

        public void WatchForSources()
        {
        }

        public SourceControlState GetSourceControlState(int sourceControlId)
        {
            return TaskRunnerContext.GetSourceControlVersionState(sourceControlId);
        }

        public ProjectState GetProjectState(int projectId)
        {
            return TaskRunnerContext.GetProjectVersionState(projectId);
        }

        public BundleState GetBundleState(int bundleId)
        {
            return TaskRunnerContext.GetBundleVersionState(bundleId);
        }
        public MachineState GetMachineState(int machineId)
        {
            return TaskRunnerContext.GetMachineState(machineId);
        }

        public void Shutdown()
        {
        }

        private static void TimerCallback(object state)
        {
            if (IsTimerRunning)
            {
                return;
            }

            IsTimerRunning = true;

            ProcessTasks();

            IsTimerRunning = false;
        }

        public static void ProcessTasks()
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

       
            TakeSources(entities);
            BuildProjects(entities);
            PackageBundles(entities);
            ScheduleAutoDeployToTest(entities);
            DeployScheduledPublications(entities);
            
        }

        private static void DeployScheduledPublications(AspNetDeployEntities entities)
        {
            List<Publication> publications = entities.Publication
                .Include("Package.BundleVersion")
                .Include("Environment")
                .Where( p => p.State == PublicationState.Queued)
                .ToList();

            List<IGrouping<BundleVersion, Publication>> groupedPublications = publications
                .GroupBy( p => p.Package.BundleVersion)
                .Where(bv => !bv.Key.IsDeleted)
                .ToList();

            groupedPublications.ForEach(group =>
            {
                TaskRunnerContext.SetBundleVersionState(group.Key.Id, BundleState.Deploying);

                try
                {
                    foreach (Publication publication in group)
                    {
                        try
                        {
                            DeploymentJob job = new DeploymentJob();
                            job.Start(
                                publication.Id,
                                machineId => TaskRunnerContext.SetMachineState(machineId, MachineState.Deploying),
                                (machineId, isSuccess) => TaskRunnerContext.SetMachineState(machineId, isSuccess ? MachineState.Idle : MachineState.Error));
                        }
                        catch (Exception e)
                        {
                            throw new AspNetDeployException("Publication failed: " + publication.Id, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    TaskRunnerContext.SetBundleVersionState(group.Key.Id, BundleState.Error);

                    Factory.GetInstance<ILoggingService>().Log(e, null);

                    if (IsExceptionCritical(e))
                    {
                        throw;
                    }
                    
                    return;
                }

                TaskRunnerContext.SetBundleVersionState(group.Key.Id, BundleState.Idle);

            });
        }


        private static void ScheduleAutoDeployToTest(AspNetDeployEntities entities)
        {

            List<BundleVersion> bundleVersions = entities.BundleVersion
                .Include("Properties")
                .Include("Packages.Publications.Environment")
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .Where(bv => !bv.IsDeleted)
                .ToList();

            List<BundleVersion> bundleVersionsWithAutoDeploy = bundleVersions
                .Where(bv => 
                    bv.GetIntProperty("AutoDeployToEnvironment") > 0 && 
                    bv.Packages.Any())
                .ToList();

            bundleVersionsWithAutoDeploy.ForEach(bundleVersion =>
            {
                int deployToEnvironmentId = bundleVersion.GetIntProperty("AutoDeployToEnvironment");

                if (bundleVersion.Packages.Any(p => p.Publications.Any(pub => pub.EnvironmentId == deployToEnvironmentId && pub.State == PublicationState.Queued)))
                {
                    return;
                }

                if (TaskRunnerContext.GetBundleVersionState(bundleVersion.Id) != BundleState.Idle)
                {
                    return;
                }

                Package latestPackage = bundleVersion.Packages.OrderByDescending( p => p.CreatedDate).First();
                List<Publication> latestPackagePublications = latestPackage.Publications.OrderByDescending( p => p.CreatedDate).ToList();

                if (latestPackagePublications.Count == 0)
                {
                    Publication publication = new Publication();
                    publication.CreatedDate = DateTime.UtcNow;
                    publication.EnvironmentId = deployToEnvironmentId;
                    publication.Package = latestPackage;
                    publication.State = PublicationState.Queued;

                    entities.Publication.Add(publication);
                }

            });

            entities.SaveChanges();
        }

        private static void PackageBundles(AspNetDeployEntities entities)
        {
            List<BundleVersion> bundleVersions = entities.BundleVersion
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .Where(bv => !bv.IsDeleted)
                .ToList();

            List<ProjectVersion> projectVersions = bundleVersions
                .SelectMany(scv => scv.ProjectVersions)
                .Where(pv => 
                        pv.ProjectType.HasFlag(ProjectType.WindowsApplication) || 
                        pv.ProjectType.HasFlag(ProjectType.Database) || 
                        pv.ProjectType.HasFlag(ProjectType.ZipArchive) || 
                        pv.ProjectType.HasFlag(ProjectType.Service) ||
                        pv.ProjectType.HasFlag(ProjectType.Console) ||
                        pv.ProjectType.HasFlag(ProjectType.Web)
                )
                .Where(pv => pv.SourceControlVersion.GetStringProperty("Revision") != pv.GetStringProperty("LastPackageRevision"))
                .ToList();

            List<BundleVersion> bundleVersionsToPack = projectVersions.SelectMany( pv => pv.BundleVersions).Distinct().ToList();

            bundleVersionsToPack.ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Packaging));

            List<BundleVersion> errorBundles = new List<BundleVersion>();

            bundleVersionsToPack.ForEach(bundleVersion =>
            {
                try
                {
                    PackageJob job = new PackageJob();
                    job.Start(bundleVersion.Id);
                }
                catch (Exception e)
                {
                    TaskRunnerContext.SetBundleVersionState(bundleVersion.Id, BundleState.Error);
                    Factory.GetInstance<ILoggingService>().Log(e, null);
                    errorBundles.Add(bundleVersion);

                    if (IsExceptionCritical(e))
                    {
                        throw;
                    }
                }

            });

            bundleVersionsToPack
                .Except(errorBundles)
                .ToList()
                .ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle));
        }

        private static void BuildProjects(AspNetDeployEntities entities)
        {
            List<BundleVersion> bundleVersions = entities.BundleVersion
                .Include("Properties")
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .ToList();

            List<ProjectVersion> projectVersions = bundleVersions
                .SelectMany( scv=> scv.ProjectVersions)
                .Where(pv => 
                    (
                        pv.ProjectType.HasFlag(ProjectType.Database) || 
                        pv.ProjectType.HasFlag(ProjectType.WindowsApplication) || 
                        pv.ProjectType.HasFlag(ProjectType.Service) ||
                        pv.ProjectType.HasFlag(ProjectType.Console) ||
                        pv.ProjectType.HasFlag(ProjectType.Web)
                    ))
                .Where(pv => pv.SourceControlVersion.GetStringProperty("Revision") != pv.GetStringProperty("LastBuildRevision"))
                .ToList();

            List<BundleVersion> affectedBundleVersions = projectVersions
                .SelectMany( pv => pv.BundleVersions)
                .Where(bv => !bv.IsDeleted)
                .ToList();

            DateTime buildStartDate = DateTime.UtcNow;

            affectedBundleVersions.ForEach(bv =>
            {
                TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Building);
                bv.SetStringProperty("LastBuildStartDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            });

            entities.SaveChanges();

            foreach (var pair in projectVersions.Select(pv => new { pv.SolutionFile, pv.SourceControlVersion }).Distinct())
            {
                try
                {
                    BuildProjectJob job = new BuildProjectJob();
                    job.Start(
                        pair.SourceControlVersion.Id, 
                        pair.SolutionFile, 
                        projectId => TaskRunnerContext.SetProjectVersionState(projectId, ProjectState.Building), 
                        (projectId, isSuccess) => TaskRunnerContext.SetProjectVersionState(projectId, isSuccess ? ProjectState.Idle : ProjectState.Error));
                }
                catch (Exception e)
                {
                    AspNetDeployException aspNetDeployException = new AspNetDeployException("Build project failed", e);
                    aspNetDeployException.Data.Add("SourceControl version Id", pair.SourceControlVersion.Id);
                    aspNetDeployException.Data.Add("Solution file", pair.SolutionFile);

                    Factory.GetInstance<ILoggingService>().Log(aspNetDeployException, null);

                    if (IsExceptionCritical(e))
                    {
                        throw;
                    }
                }
            }

            affectedBundleVersions.ForEach(bv =>
            {
                TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle);
                bv.SetStringProperty("LastBuildDuration", (DateTime.UtcNow - buildStartDate).TotalSeconds.ToString(CultureInfo.InvariantCulture));
            });

            entities.SaveChanges();
        }

        private static void TakeSources(AspNetDeployEntities entities)
        {
            List<SourceControlVersion> sourceControlVersions = entities.SourceControlVersion
                .Include("ProjectVersions")
                .Include("Properties")
                .Include("ProjectVersions.BundleVersions")
                .Include("SourceControl.Properties")
                .ToList();

            IList<BundleVersion> bundleVersions = sourceControlVersions
                .SelectMany(scv => scv.ProjectVersions)
                .SelectMany(pv => pv.BundleVersions)
                .Where(bv => !bv.IsDeleted)
                .ToList();

            sourceControlVersions
                .ForEach(sourceControlVersion =>
                {
                    TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Loading);

                    sourceControlVersion.ProjectVersions
                        .SelectMany( pv => pv.BundleVersions)
                        .Where(bv => !bv.IsDeleted)
                        .ToList()
                        .ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Loading));

                    try
                    {
                        (new SourceControlJob(sourceControlVersion.Id)).Start();
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Idle);
                    }
                    catch (Exception e)
                    {
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Error);
                        Factory.GetInstance<ILoggingService>().Log(new AspNetDeployException("Take sources failed: " + sourceControlVersion.Id, e), null);
                        
                        if (IsExceptionCritical(e))
                        {
                            throw;
                        }
                    }

                    sourceControlVersion.ProjectVersions
                        .SelectMany(pv => pv.BundleVersions)
                        .Where(bv => !bv.IsDeleted)
                        .ToList()
                        .ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle));

                });
        }

        private static bool IsExceptionCritical(Exception exception)
        {
            if (exception is ThreadAbortException)
            {
                return true;
            }

            if (exception is OutOfMemoryException)
            {
                return true;
            }

            if (exception is AppDomainUnloadedException)
            {
                return true;
            }

            if (exception is BadImageFormatException)
            {
                return true;
            }

            if (exception is CannotUnloadAppDomainException)
            {
                return true;
            }

            if (exception is InvalidProgramException)
            {
                return true;
            }

            return false;
        }

    }

    
}
