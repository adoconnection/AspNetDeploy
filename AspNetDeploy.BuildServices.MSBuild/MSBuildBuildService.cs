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
        private readonly INugetPackageManager nugetPackageManager;

        public MSBuildBuildService(INugetPackageManager nugetPackageManager)
        {
            this.nugetPackageManager = nugetPackageManager;
        }

        public BuildSolutionResult Build(string solutionFilePath, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            nugetPackageManager.RestorePackages(solutionFilePath);

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
                new MSBuildLogger(projectBuildStarted, projectBuildComplete, errorLogger)
            };

            BuildResult buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            return new BuildSolutionResult
            {
                IsSuccess = buildResult.OverallResult == BuildResultCode.Success
            };
        }
    }
}
