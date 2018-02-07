using System;
using System.Diagnostics;
using System.IO;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.BuildServices.DotnetCore
{
    public class DotnetCoreBuildService : IBuildService
    {
        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            var fullPath = Path.Combine(sourcesFolder, projectVersion.ProjectFile);

            DoDotnet(Path.GetDirectoryName(fullPath), "restore");
            DoDotnet(Path.GetDirectoryName(fullPath), "publish -c Release");

            return new BuildSolutionResult()
            {
                IsSuccess = true
            };
        }

        private static void DoDotnet(string workingDirectory, string arguments)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = workingDirectory;

            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = arguments;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                DotnetCoreBuildServiceException exception = new DotnetCoreBuildServiceException("Nuget returned: " + process.ExitCode);
                exception.Data.Add("Output", output);

                throw exception;
            }
        }
    }
}