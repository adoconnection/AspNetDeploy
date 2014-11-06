using System.Collections.Generic;
using AspNetDeploy.Contracts;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildBuildService : IBuildService
    {
        public void Build(string solutionFilePath, IContinuousIntegrationLogger logger)
        {
            ProjectCollection projectCollection = new ProjectCollection();

            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"}, 
                {"Platform", "Any CPU"}
            };

            BuildRequestData buildRequestData = new BuildRequestData(solutionFilePath, globalProperty, null, new[] { "Build" }, null);

            BuildParameters buildParameters = new BuildParameters(projectCollection);
            buildParameters.Loggers = new List<ILogger> {new MSBuildLogger(logger)};

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);
        }
    }
}
