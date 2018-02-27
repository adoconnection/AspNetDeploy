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

            projectBuildStarted(fullPath);
            string output;

            if (DoDotnet(Path.GetDirectoryName(fullPath), "restore", out output) != 0)
            {
                errorLogger(fullPath, new DotnetCoreBuildServiceException(output));
                projectBuildComplete(fullPath, false, output);

                return new BuildSolutionResult()
                {
                    IsSuccess = false
                };
            }

            if (DoDotnet(Path.GetDirectoryName(fullPath), "publish -c Release", out output) != 0)
            {
                errorLogger(fullPath, new DotnetCoreBuildServiceException(output));
                projectBuildComplete(fullPath, false, output);

                return new BuildSolutionResult()
                {
                    IsSuccess = false
                };
            }

            projectBuildComplete(fullPath, true, null);

            return new BuildSolutionResult()
            {
                IsSuccess = true
            };
        }

        private static int DoDotnet(string workingDirectory, string arguments, out string output)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = workingDirectory;

            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = arguments;

            process.Start();

            output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return process.ExitCode;
        }
    }
}