using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public class ThreadTaskRunner : ITaskRunner
    {
        private static ISourceControlRepositoryFactory SourceControlRepositoryFactory;

        private static ConcurrentDictionary<int, SourceControlJob> SourceControlWorkers = new ConcurrentDictionary<int, SourceControlJob>();
        private static ConcurrentDictionary<int, BackgroundWorker> ProjectWorkers = new ConcurrentDictionary<int, BackgroundWorker>();

        private readonly Timer Timer = new Timer(TimerCallback);

        public ThreadTaskRunner(ISourceControlRepositoryFactory sourceControlRepositoryFactory)
        {
            SourceControlRepositoryFactory = sourceControlRepositoryFactory;
        }

        public void Initialize()
        {
            Timer.Change(new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 30));
        }

        public void WatchForSources()
        {
        }

        public SourceControlState GetSourceControlState(int sourceControlId)
        {
            return SourceControlWorkers.GetOrAdd(sourceControlId, i => new SourceControlJob(SourceControlRepositoryFactory)).Status;
        }

        public ProjectState GetProjectState(int projectId)
        {
            return ProjectState.Idle;
        }

        public void Shutdown()
        {
        }

        private static void TimerCallback(object state)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            List<SourceControl> sourceControls = entities.SourceControl
                .Include("Properties")
                .ToList();

            foreach (SourceControl sourceControl in sourceControls)
            {
                SourceControlJob sourceControlJob = SourceControlWorkers.GetOrAdd(sourceControl.Id, i => new SourceControlJob(SourceControlRepositoryFactory));

                if (sourceControlJob.Status == SourceControlState.Idle)
                {
                    sourceControlJob.Start(sourceControl);
                }
            }
        }

    }
}
