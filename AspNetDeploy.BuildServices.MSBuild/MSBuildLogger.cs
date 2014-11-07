using System;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildLogger : ILogger
    {
        private readonly Action<string> projectBuildStarted;
        private readonly Action<string, bool> projectBuildComplete;
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        public MSBuildLogger(Action<string> projectBuildStarted, Action<string, bool> projectBuildComplete)
        {
            this.projectBuildStarted = projectBuildStarted;
            this.projectBuildComplete = projectBuildComplete;
        }

        public void Initialize(IEventSource eventSource)
        {
            this.Verbosity = LoggerVerbosity.Diagnostic;
            eventSource.ProjectStarted += EventSourceOnProjectStarted ;
            eventSource.ProjectFinished += EventSourceOnProjectFinished;
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
        {
            if (projectStartedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            this.projectBuildStarted(projectStartedEventArgs.ProjectFile);
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            if (projectFinishedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            this.projectBuildComplete(projectFinishedEventArgs.ProjectFile, projectFinishedEventArgs.Succeeded);

        }

        public void Shutdown()
        {
            
        }


    }
}