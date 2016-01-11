using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using Exceptions;
using ObjectFactory;
using ThreadHostedTaskRunner.Jobs;

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

        public SourceControlState GetSourceControlVersionState(int sourceControlId)
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
       
            ArchiveSources(entities);
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

                    if (e.IsCritical())
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
                    bv.Packages.Any() &&
                    bv.ProjectVersions.All(pv => pv.SourceControlVersion.ArchiveState == SourceControlVersionArchiveState.Normal))
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
                .Where(pv => 
                    pv.SourceControlVersion.GetStringProperty("Revision") != pv.GetStringProperty("LastPackageRevision"))
                .ToList();

            List<BundleVersion> bundleVersionsToPack = projectVersions
                .SelectMany( pv => pv.BundleVersions)
                .Distinct()
                .Where( bv => bv.ProjectVersions.All( 
                    pv => pv.SourceControlVersion.ArchiveState == SourceControlVersionArchiveState.Normal && 
                    pv.GetStringProperty("LastBuildResult") != "Error"))
                .ToList();

            List<BundleVersion> errorBundles = new List<BundleVersion>();

            bundleVersionsToPack.ForEach(bundleVersion =>
            {
                /*if (bundleVersion.ProjectVersions.Any( pv => 
                    TaskRunnerContext.GetProjectVersionState(pv.Id) != ProjectState.Idle ||
                    pv.GetStringProperty("LastBuildResult") != "Done"))
                {
                    return;
                }*/

                TaskRunnerContext.SetBundleVersionState(bundleVersion.Id, BundleState.Packaging);

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

                    if (e.IsCritical())
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

            foreach (var projectVersion in projectVersions.Where(pv => pv.GetStringProperty("LastBuildResult") == "Error"))
            {
                TaskRunnerContext.SetProjectVersionState(projectVersion.Id, ProjectState.Error);
            }

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

            ProjectsBuildStrategy buildStrategy = new ProjectsBuildStrategy(entities);
            buildStrategy.Build(projectVersions);

            foreach (ProjectVersion projectVersion in projectVersions)
            {
                if (projectVersion.GetStringProperty("LastBuildRevision") != projectVersion.SourceControlVersion.GetStringProperty("Revision"))
                {
                    projectVersion.SetStringProperty("LastBuildRevision", projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                }
            }

            affectedBundleVersions.ForEach(bv =>
            {
                if (bv.ProjectVersions.Any(pv => pv.GetStringProperty("LastBuildResult") == "Error"))
                {
                    TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Error);
                    return;
                }

                TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle);

                if (bv.ProjectVersions.All(pv => TaskRunnerContext.GetProjectVersionState(pv.Id) == ProjectState.Idle))
                {
                    bv.SetStringProperty("LastBuildDuration", (DateTime.UtcNow - buildStartDate).TotalSeconds.ToString(CultureInfo.InvariantCulture));
                }
            });

            entities.SaveChanges();
        }

        private static void ArchiveSources(AspNetDeployEntities entities)
        {
            List<SourceControlVersion> sourceControlVersions = entities.SourceControlVersion
                .Include("ProjectVersions")
                .Include("Properties")
                .Include("ProjectVersions.BundleVersions")
                .Include("SourceControl.Properties")
                .Where(scv => scv.ArchiveState == SourceControlVersionArchiveState.Archiving)
                .ToList();

            sourceControlVersions
                .ForEach(sourceControlVersion =>
                {
                    TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Archiving);

                    try
                    {
                        (new SourceControlJob(sourceControlVersion.Id)).Archive();
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Idle);
                    }
                    catch (Exception e)
                    {
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Error);
                        Factory.GetInstance<ILoggingService>().Log(new AspNetDeployException("Archive sources failed: " + sourceControlVersion.Id, e), null);

                        if (e.IsCritical())
                        {
                            throw;
                        }
                    }
                });
        }

        private static void TakeSources(AspNetDeployEntities entities)
        {
            List<SourceControlVersion> sourceControlVersions = entities.SourceControlVersion
                .Include("ProjectVersions")
                .Include("Properties")
                .Include("ProjectVersions.BundleVersions")
                .Include("SourceControl.Properties")
                .Where( scv => scv.ArchiveState == SourceControlVersionArchiveState.Normal)
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
                        (new SourceControlJob(sourceControlVersion.Id)).UpdateAndParse();
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Idle);
                    }
                    catch (Exception e)
                    {
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Error);
                        Factory.GetInstance<ILoggingService>().Log(new AspNetDeployException("Take sources failed: " + sourceControlVersion.Id, e), null);
                        
                        if (e.IsCritical())
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
    }

    
}
