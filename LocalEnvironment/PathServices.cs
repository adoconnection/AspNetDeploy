using System.Configuration;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;

namespace LocalEnvironment
{
    public class PathServices : IPathServices
    {
        public string GetSourceControlVersionPath(int sourceControlId, int sourceControlVersionId)
        {
            return this.GetWorkingFolderPath(string.Format(@"Sources\{0}\{1}", sourceControlId, sourceControlVersionId));
        }

        public string GetBundlePackagePath(int bundleId, int packageId)
        {
            return this.GetWorkingFolderPath(string.Format(@"Packages\package-{0}-{1}.zip", bundleId, packageId));
        }

        public string GetProjectPackagePath(int projectId, string revisionId)
        {
            return this.GetWorkingFolderPath(string.Format(@"Packages\project-{0}-{1}.zip", projectId, revisionId));
        }

        public string GetNugetPath()
        {
            return this.GetWorkingFolderPath(@"NuGet\NuGet.exe");
        }

        public string GetNpmPath()
        {
            return ConfigurationManager.AppSettings["Settings.NpmBinary"];
        }

        public string GetMSBuildPath()
        {
            return @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"; //Затычка
        }

        public string GetRootCertificatePath(bool isPfx = true)
        {
            if (isPfx)
            {
                return this.GetWorkingFolderPath("Certificates\\root.pfx");
            }

            return this.GetWorkingFolderPath("Certificates\\root.cer");
        }

        public string GetClientCertificatePath()
        {
            return this.GetWorkingFolderPath("Certificates\\client.pfx");
        }

        public string GetMachineCertificatePath(bool isRoot = false)
        {
            if (isRoot)
            {
                return this.GetWorkingFolderPath(Path.Combine("MachineAgent", "Template", "Certificates", "rootCertificate.crt"));
            }

            return this.GetWorkingFolderPath(Path.Combine("MachineAgent", "Template", "Certificates", "machineCertificate.pfx"));
        }

        private string GetWorkingFolderPath(string path)
        {
            return Path.Combine(this.GetWorkingFolder(), path);
        }

        private string GetWorkingFolder()
        {
            string workingFolder = ConfigurationManager.AppSettings["Settings.WorkingFolder"];

            if (string.IsNullOrWhiteSpace(workingFolder))
            {
                throw new AspNetDeployException("Settings.WorkingFolder in AppSettings is not defined");
            }

            return workingFolder;
        }
    }
}
