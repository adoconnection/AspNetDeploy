using System;
using System.Diagnostics;
using System.IO;
using AspNetDeploy.Contracts;

namespace BuildServices.NuGet
{
    public class NugetPackageManager : INugetPackageManager
    {
        private readonly IPathServices pathServices;

        public NugetPackageManager(IPathServices pathServices)
        {
            this.pathServices = pathServices;
        }

        public void RestoreSolutionPackages(string solutionFile)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(solutionFile);

            process.StartInfo.FileName = this.pathServices.GetNugetPath();
            process.StartInfo.Arguments = string.Format(
                "restore \"{0}\"",
                solutionFile);

            process.Start();

            process.StandardOutput.ReadToEnd();

            process.WaitForExit();
        }

        public void RestoreProjectPackages(string projectFile, string solutionDirectory)
        {
            string directoryName = Path.GetDirectoryName(projectFile);
            string packageFile = Path.Combine(directoryName, "packages.config");

            if (!File.Exists(packageFile))
            {
                return;
            }

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = solutionDirectory;

            process.StartInfo.FileName = this.pathServices.GetNugetPath();
            process.StartInfo.Arguments = string.Format(
                "restore \"{0}\" -solutiondirectory \"{1}\"",
                packageFile,
                directoryName);

            process.Start();

            process.WaitForExit();
        }

    }
}
