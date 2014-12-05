using System;
using System.Collections.Generic;
using System.IO;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildBuildService : IBuildService
    {
        public BuildSolutionResult Build(string solutionFilePath, Action<string> projectBuildStarted, Action<string, bool> projectBuildComplete)
        {
            ProjectCollection projectCollection = new ProjectCollection();

            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"}, 
                {"Platform", "Any CPU"}
            };

            BuildRequestData buildRequestData = new BuildRequestData(solutionFilePath, globalProperty, null, new[] { "Rebuild" }, null);

            BuildParameters buildParameters = new BuildParameters(projectCollection);
            buildParameters.MaxNodeCount = 1;
            buildParameters.Loggers = new List<ILogger>
            {
                new NugetPackageRestorer(Path.GetDirectoryName(solutionFilePath)),
                new MSBuildLogger(projectBuildStarted, projectBuildComplete)
            };

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            return new BuildSolutionResult
            {
                IsSuccess = buildResult.OverallResult == BuildResultCode.Success
            };
        }
    }
}
