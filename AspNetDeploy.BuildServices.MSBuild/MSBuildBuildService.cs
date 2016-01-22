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

        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            nugetPackageManager.RestoreSolutionPackages(Path.Combine(sourcesFolder, projectVersion.SolutionFile));
            return this.BuildInternal(Path.Combine(sourcesFolder, projectVersion.ProjectFile), projectBuildStarted, projectBuildComplete, errorLogger);
        }

       /* public BuildSolutionResult Build(string solutionFile, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            nugetPackageManager.RestoreSolutionPackages(solutionFile);
            return this.BuildInternal(solutionFile, projectBuildStarted, projectBuildComplete, errorLogger);
        }
        */

        private BuildSolutionResult BuildInternal(string targetFile, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            ProjectCollection projectCollection = new ProjectCollection();

            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"},
                //{"Platform", "Any CPU"}
            };

            BuildRequestData buildRequestData = new BuildRequestData(
                targetFile,
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
