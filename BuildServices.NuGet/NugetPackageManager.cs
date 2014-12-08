using System.Diagnostics;
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

        public void RestorePackages(string packagesConfigPath, string solutionDirectory)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.FileName = this.pathServices.GetNugetPath();
            process.StartInfo.Arguments = string.Format(
                "install \"{0}\" -source \"{1}\" -solutionDir \"{2}\"",
                packagesConfigPath,
                "https://www.nuget.org/api/v2/",
                solutionDirectory);

            process.Start();

            process.WaitForExit();
        }
    }
}
