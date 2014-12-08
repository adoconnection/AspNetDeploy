using System;
using System.ComponentModel;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildLogger : ILogger
    {
        private readonly Action<string> projectBuildStarted;
        private readonly Action<string, bool, string> projectBuildComplete;
        private readonly Action<string, string, string, int, int, string> errorLogger;
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        public MSBuildLogger(Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            this.projectBuildStarted = projectBuildStarted;
            this.projectBuildComplete = projectBuildComplete;
            this.errorLogger = errorLogger;
        }

        public void Initialize(IEventSource eventSource)
        {
            this.Verbosity = LoggerVerbosity.Diagnostic;
            eventSource.ProjectStarted += EventSourceOnProjectStarted;
            eventSource.ProjectFinished += EventSourceOnProjectFinished;
            eventSource.ErrorRaised += this.EventSourceOnErrorRaised;
        }

        void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            if (this.errorLogger == null)
            {
                return;
            }

            if (e.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            this.errorLogger(e.ProjectFile, e.File, e.Code, e.LineNumber, e.ColumnNumber, e.Message);
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

            this.projectBuildComplete(projectFinishedEventArgs.ProjectFile, projectFinishedEventArgs.Succeeded, projectFinishedEventArgs.Message);

        }

        public void Shutdown()
        {
            
        }


    }
}