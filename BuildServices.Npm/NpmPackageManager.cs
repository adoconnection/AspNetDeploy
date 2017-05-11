using System.Diagnostics;
using AspNetDeploy.Contracts;

namespace BuildServices.Npm
{
    public class NpmPackageManager : INpmPackageManager
    {
        private readonly IPathServices pathServices;

        public NpmPackageManager(IPathServices pathServices)
        {
            this.pathServices = pathServices;
        }

        public void RestorePackages(string directory)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = directory;

            process.StartInfo.FileName = this.pathServices.GetNugetPath();
            process.StartInfo.Arguments = "install";

            process.Start();

            process.WaitForExit();
        }
    }
}