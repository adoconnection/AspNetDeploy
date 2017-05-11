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
        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, string> errorLogger)
        {
            string targetFile = Path.Combine(sourcesFolder, projectVersion.ProjectFile);

            // this interface may be useful later
            IPackageManager packageManager = new NpmPackageManager();
            packageManager.RestorePackages(sourcesFolder);

            projectBuildStarted(projectVersion.ProjectFile);
            
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
                    throw new Exception(error);
                }

                projectBuildComplete(projectVersion.ProjectFile, true, null);

                return new BuildSolutionResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                errorLogger(projectVersion.ProjectFile, "Message: " + ex.Message);
                projectBuildComplete(projectVersion.ProjectFile, false, ex.Message);

                return new BuildSolutionResult()
                {
                    IsSuccess = false
                };
            }
        }
    }
}