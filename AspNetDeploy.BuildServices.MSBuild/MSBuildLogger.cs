using System;
using System.ComponentModel;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildLogger : ILogger
    {
        private readonly Action<string> projectBuildStarted;
        private readonly Action<string, bool, string> projectBuildComplete;
        private readonly Action<string, string> errorLogger;
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        public MSBuildLogger(Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string> errorLogger)
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

            this.errorLogger(e.ProjectFile, string.Format("File: {0}, code: {1}, lineNumber: {2}, columnNumber: {3}, message: {4}", e.File, e.Code, e.LineNumber, e.ColumnNumber, e.Message));
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
        {
            if (projectStartedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (this.projectBuildStarted != null)
            {
                this.projectBuildStarted(projectStartedEventArgs.ProjectFile);
            }
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            if (projectFinishedEventArgs.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (this.projectBuildComplete != null)
            {
                this.projectBuildComplete(projectFinishedEventArgs.ProjectFile, projectFinishedEventArgs.Succeeded, projectFinishedEventArgs.Message);
            }
        }

        public void Shutdown()
        {
            
        }


    }
}