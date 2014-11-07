using System.Diagnostics;

namespace BuildServices.NuGet
{
    public class NugetPackageManager
    {
        public void RestorePackages(string packagesConfigPath, string solutionDirectory)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.StartInfo.FileName = @"H:\AspNetDeployWorkingFolder\NuGet\NuGet.exe";
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
