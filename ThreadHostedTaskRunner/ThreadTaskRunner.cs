using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Microsoft.VisualBasic;
using ThreadHostedTaskRunner.Jobs;

namespace ThreadHostedTaskRunner
{
    public class ThreadTaskRunner : ITaskRunner
    {
        private readonly Timer Timer = new Timer(TimerCallback);
        private static bool IsTimerRunning = false;

        public void Initialize()
        {
            Timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 10));
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
            
        }

        private static void PackageBundles(AspNetDeployEntities entities)
        {
            List<BundleVersion> bundleVersions = entities.BundleVersion
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .ToList();

            List<ProjectVersion> projectVersions = bundleVersions
                .SelectMany(scv => scv.ProjectVersions)
                .Where(pv => 
                        pv.ProjectType.HasFlag(ProjectType.WindowsApplication) || 
                        pv.ProjectType.HasFlag(ProjectType.Service) ||
                        pv.ProjectType.HasFlag(ProjectType.Console) ||
                        pv.ProjectType.HasFlag(ProjectType.Web)
                )
                .Where(pv => pv.SourceControlVersion.GetStringProperty("Revision") != pv.GetStringProperty("LastPackageRevision"))
                .ToList();

            List<BundleVersion> bundleVersionsToPack = projectVersions.SelectMany( pv => pv.BundleVersions).Distinct().ToList();

            bundleVersionsToPack.ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Packaging));

            bundleVersionsToPack.ForEach(bundleVersion =>
            {
                PackageJob job = new PackageJob();
                job.Start(bundleVersion.Id);

            });

            bundleVersionsToPack.ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle));
        }

        private static void BuildProjects(AspNetDeployEntities entities)
        {
            List<BundleVersion> bundleVersions = entities.BundleVersion
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .ToList();

            List<ProjectVersion> projectVersions = bundleVersions
                .SelectMany( scv=> scv.ProjectVersions)
                .Where(pv => 
                    (
                        pv.ProjectType.HasFlag(ProjectType.WindowsApplication) || 
                        pv.ProjectType.HasFlag(ProjectType.Service) ||
                        pv.ProjectType.HasFlag(ProjectType.Console) ||
                        pv.ProjectType.HasFlag(ProjectType.Web)
                    ))
                .Where(pv => 
                    pv.SourceControlVersion.GetStringProperty("Revision") != pv.GetStringProperty("LastBuildRevision"))
                .ToList();

            bundleVersions.ForEach( bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Building));

            foreach (var pair in projectVersions.Select(pv => new { pv.SolutionFile, pv.SourceControlVersion }).Distinct())
            {
                BuildProjectJob job = new BuildProjectJob();
                job.Start(
                    pair.SourceControlVersion.Id, 
                    pair.SolutionFile, 
                    (projectId, isSuccess) => TaskRunnerContext.SetProjectVersionState(projectId, isSuccess ? ProjectState.Idle : ProjectState.Error));
            }

            bundleVersions.ForEach(bv => TaskRunnerContext.SetBundleVersionState(bv.Id, BundleState.Idle));
        }

        private static void TakeSources(AspNetDeployEntities entities)
        {
            List<SourceControlVersion> sourceControlVersions = entities.SourceControlVersion
                .Include("ProjectVersions")
                .Include("Properties")
                .Include("ProjectVersions.BundleVersions")
                .Include("SourceControl.Properties")
                .ToList();

            IList<BundleVersion> bundleVersions = sourceControlVersions.SelectMany( scv => scv.ProjectVersions).SelectMany( pv => pv.BundleVersions).ToList();

            foreach (BundleVersion bundleVersion in bundleVersions)
            {
                TaskRunnerContext.SetBundleVersionState(bundleVersion.Id, BundleState.Loading);
            }

            sourceControlVersions
                .ForEach(sourceControlVersion =>
                {
                    TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Loading);

                    try
                    {
                        (new SourceControlJob(sourceControlVersion.Id)).Start();
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Idle);
                    }
                    catch (Exception e)
                    {
                        TaskRunnerContext.SetSourceControlVersionState(sourceControlVersion.Id, SourceControlState.Error);
                    }
                });

            foreach (BundleVersion bundleVersion in bundleVersions)
            {
                TaskRunnerContext.SetBundleVersionState(bundleVersion.Id, BundleState.Idle);
            }
        }

    }
}
