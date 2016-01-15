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

        public BuildSolutionResult Build(string projectOrSolutionFilePath, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            nugetPackageManager.RestorePackages(projectOrSolutionFilePath);

            ProjectCollection projectCollection = new ProjectCollection();

            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"}, 
                //{"Platform", "Any CPU"}
            };

            BuildRequestData buildRequestData = new BuildRequestData(
                projectOrSolutionFilePath, 
                globalProperty, 
                this.LatestToolsVersion(), 
                new[]
                {
                    "Rebuild"
                }, 
                null);

            BuildParameters buildParameters = new BuildParameters(projectCollection);
            buildParameters.MaxNodeCount = 4;
            
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

        private string LatestToolsVersion()
        {
            if (Directory.Exists(@"C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v14.0"))
            {
                return "14.0";
            }

            if (Directory.Exists(@"C:\Program Files (x86)\MSBuild\Microsoft\VisualStudio\v12.0"))
            {
                return "12.0";
            }

            return null;
        }
    }
}
