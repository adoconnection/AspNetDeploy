using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using AspNetDeploy.BuildServices.NuGet;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using BuildServices.NuGet;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class MSBuildBuildService : IBuildService
    {
        private readonly INugetPackageManager nugetPackageManager;
        private readonly IPathServices pathServices;

        public MSBuildBuildService(IPathServices pathServices)
        {
            this.nugetPackageManager = new NugetPackageManager(pathServices);
            this.pathServices = pathServices;
        }

        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            this.nugetPackageManager.RestoreSolutionPackages(Path.Combine(sourcesFolder, projectVersion.SolutionFile));
            return this.BuildInternal(Path.Combine(sourcesFolder, projectVersion.ProjectFile), projectBuildStarted, projectBuildComplete, errorLogger);
        }

       /* public BuildSolutionResult Build(string solutionFile, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string, string, int, int, string> errorLogger)
        {
            nugetPackageManager.RestoreSolutionPackages(solutionFile);
            return this.BuildInternal(solutionFile, projectBuildStarted, projectBuildComplete, errorLogger);
        }
        */

        private BuildSolutionResult BuildInternal(string targetFile, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            projectBuildStarted(targetFile);
            string output;

            if (DoMSBuild(this.pathServices.GetMSBuildPath(), targetFile, out output) != 0)
            {
                errorLogger(targetFile, new MSBuildException(output));
                projectBuildComplete(targetFile, false, output);

                return new BuildSolutionResult
                {
                    IsSuccess = false
                };
            }

            projectBuildComplete(targetFile, true, null);

            return new BuildSolutionResult
            {
                IsSuccess = true
            };
        }

        private static int DoMSBuild(string toolPath, string targetFile, out string output)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(targetFile);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = toolPath;
            process.StartInfo.Arguments = string.Format("\"{0}\" /t:Restore;Clean;Rebuild -p:Configuration=Release", targetFile);

            process.Start();
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode;
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
