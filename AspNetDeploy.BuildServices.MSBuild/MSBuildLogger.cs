using System;
using AspNetDeploy.Contracts;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildLogger : ILogger
    {
        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }

        private readonly int sourceControlId;
        private readonly IContinuousIntegrationLogger logger;

        public MSBuildLogger(IContinuousIntegrationLogger logger)
        {
            this.logger = logger;
        }

        public void Initialize(IEventSource eventSource)
        {
            this.Verbosity = LoggerVerbosity.Diagnostic;
            eventSource.ProjectStarted += EventSourceOnProjectStarted ;
            eventSource.ProjectFinished += EventSourceOnProjectFinished;
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs projectStartedEventArgs)
        {
            if (projectStartedEventArgs.ProjectFile.EndsWith(".sln"))
            {
                logger.SolutionBuildStarted(projectStartedEventArgs.ProjectFile);
            }

        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs projectFinishedEventArgs)
        {
            if (projectFinishedEventArgs.ProjectFile.EndsWith(".sln"))
            {
                if (projectFinishedEventArgs.Succeeded)
                {
                    logger.SolutionBuildComplete(projectFinishedEventArgs.ProjectFile);
                }
                else
                {
                    logger.SolutionBuildFailed(projectFinishedEventArgs.ProjectFile);
                }
            }

        }

        public void Shutdown()
        {
            
        }


    }
}