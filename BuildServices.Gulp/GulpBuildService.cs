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
        private readonly INpmPackageManager npmPackageManager;

        public GulpBuildService(IPathServices pathServices)
        {
            this.npmPackageManager = new NpmPackageManager(pathServices);
        }

        public BuildSolutionResult Build(string sourcesFolder, ProjectVersion projectVersion, Action<string> projectBuildStarted, Action<string, bool, string> projectBuildComplete, Action<string, Exception> errorLogger)
        {
            string targetFile = Path.Combine(sourcesFolder, projectVersion.ProjectFile);

            this.npmPackageManager.RestorePackages(sourcesFolder);

            projectBuildStarted(targetFile);
            
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

                projectBuildComplete(targetFile, true, null);

                return new BuildSolutionResult()
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                GulpException exception = new GulpException("Build failed", ex);
                exception.Data.Add("sourcesFolder", sourcesFolder);
                exception.Data.Add("targetFile", targetFile);

                errorLogger(targetFile, exception);
                projectBuildComplete(targetFile, false, ex.Message);

                return new BuildSolutionResult()
                {
                    IsSuccess = false
                };
            }
        }
    }
}