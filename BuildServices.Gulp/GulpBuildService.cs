using System;
using System.Diagnostics;
using System.IO;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using BuildServices.Npm;

namespace BuildServices.Gulp
{
    public class GulpBuildService : IBuildService
    {
        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion,
            Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete,
            Action<string, string> errorLogger)
        {
            // this interface may be useful later
            IPackageManager packageManager = new NpmPackageManager();
            packageManager.RestorePackages(sourcesFolder);

            return this.BuildInternal(sourcesFolder, Path.Combine(sourcesFolder, projectVersion.ProjectFile), errorLogger);
        }

        private BuildSolutionResult BuildInternal(string sourcesFolder, string targetFile, Action<string, string> errorLogger)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = sourcesFolder;

                process.StartInfo.FileName = Path.Combine(sourcesFolder, @"node_modules\.bin\gulp.cmd");
                process.StartInfo.Arguments = string.Format("--gulpfile {0}", targetFile);

                process.Start();

                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    errorLogger(targetFile, string.Format("Message: {0}, sources folder: {1}, target file: {2}", error, sourcesFolder, targetFile));

                    return new BuildSolutionResult()
                    {
                        IsSuccess = false
                    };
                }
            }
            catch (Exception ex)
            {
                errorLogger(targetFile, string.Format("Message: {0}, sources folder: {1}, target file: {2}", ex.Message, sourcesFolder, targetFile));

                return new BuildSolutionResult()
                {
                    IsSuccess = false
                };
            }

            return new BuildSolutionResult()
            {
                IsSuccess = true
            };
        }
    }
}