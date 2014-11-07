using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public class ThreadTaskRunner : ITaskRunner
    {
        private readonly Timer Timer = new Timer(TimerCallback);
        private static bool IsTimerRunning = false;

        public void Initialize()
        {
            Timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 30));
        }

        public void WatchForSources()
        {
        }

        public SourceControlState GetSourceControlState(int sourceControlId)
        {
            return TaskRunnerContext.GetSourceControlState(sourceControlId);
        }

        public ProjectState GetProjectState(int projectId)
        {
            return TaskRunnerContext.GetProjectState(projectId);
        }

        public BundleState GetBundleState(int bundleId)
        {
            return TaskRunnerContext.GetBundleState(bundleId);
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

        private static void ProcessTasks()
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            ProcessProjects(entities);
            ProcessSourceControls(entities);
        }

        private static void ProcessProjects(AspNetDeployEntities entities)
        {
            List<Bundle> bundles = entities.Bundle
                .Include("Projects.SourceControl.Properties")
                .ToList();

            List<Project> projectsToBuild = new List<Project>();

            Parallel.ForEach(bundles, bundle =>
            {
                if (bundle.Projects.Any(p => TaskRunnerContext.GetProjectState(p.Id) != ProjectState.Idle))
                {
                    return;
                }

                if (bundle.Projects.Any(p => TaskRunnerContext.GetSourceControlState(p.SourceControl.Id) != SourceControlState.Idle))
                {
                    return;
                }

                if (TaskRunnerContext.GetBundleState(bundle.Id) == BundleState.Idle)
                {
                    foreach (SourceControl sourceControl in bundle.Projects.Select( p => p.SourceControl))
                    {
                        if (sourceControl.GetStringProperty("Revision") != sourceControl.GetStringProperty("LastBuildRevision"))
                        {
                            foreach (Project project in sourceControl.Projects)
                            {
                                TaskRunnerContext.NeedToBuildProject(project.Id, true);
                            }
                        }
                    }

                    foreach (Project project in bundle.Projects.Where( p => TaskRunnerContext.NeedToBuildProject(p.Id)))
                    {
                        TaskRunnerContext.SetProjectState(project.Id, ProjectState.QueuedToBuild);

                        foreach (Bundle projectBundle in project.Bundles)
                        {
                            TaskRunnerContext.SetBundleState(projectBundle.Id, BundleState.BuildQueued);
                        }

                        projectsToBuild.Add(project);
                    }
                }
            });

            var pairs = projectsToBuild.Select(p => new {p.SourceControl, p.SolutionFile, p.Bundles}).Distinct().ToList();

            foreach (var pair in pairs)
            {
                BuildProjectJob job = new BuildProjectJob();
                job.Start(pair.SourceControl.Id, pair.SolutionFile);

                foreach (Bundle bundle in pair.Bundles)
                {
                    TaskRunnerContext.SetBundleState(bundle.Id, BundleState.Building);
                }
            }
        }

        private static void ProcessSourceControls(AspNetDeployEntities entities)
        {
            List<SourceControl> sourceControls = entities.SourceControl
                .Include("Properties")
                .Include("Projects.Bundles")
                .ToList();

            Parallel.ForEach(sourceControls, sourceControl =>
            {
                if (sourceControl.Projects.Any(p => TaskRunnerContext.GetProjectState(p.Id) != ProjectState.Idle))
                {
                    return;
                }

                if (sourceControl.Projects.SelectMany(p => p.Bundles).Distinct().Any(b => TaskRunnerContext.GetBundleState(b.Id) != BundleState.Idle))
                {
                    return;
                }

                if (TaskRunnerContext.GetSourceControlState(sourceControl.Id) == SourceControlState.Idle)
                {
                    TaskRunnerContext.SetSourceControlState(sourceControl.Id, SourceControlState.Loading);
                    SourceControlJob job = new SourceControlJob();
                    job.Start(sourceControl.Id);
                }
            });
        }
    }
}
