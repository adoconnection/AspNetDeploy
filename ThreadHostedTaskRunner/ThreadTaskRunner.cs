using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner
{
    public class ThreadTaskRunner : ITaskRunner
    {
        private readonly Timer Timer = new Timer(TimerCallback);


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
            AspNetDeployEntities entities = new AspNetDeployEntities();

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

                if (sourceControl.Projects.SelectMany( p => p.Bundles).Distinct().Any(b => TaskRunnerContext.GetBundleState(b.Id) != BundleState.Idle))
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


            List<Bundle> bundles = entities.Bundle
                .Include("Projects.SourceControl")
                .ToList();

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
                    
                }


            });
        }

    }
}
