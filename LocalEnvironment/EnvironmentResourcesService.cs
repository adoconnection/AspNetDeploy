using System;
using System.IO;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;

namespace LocalEnvironment
{
    public class EnvironmentResourcesService : IEnvironmentResourcesService
    {
        public void InitializeWorkingFolder(string workingFolder)
        {
            this.CreateRootFolder(workingFolder);
            this.PlaceNugetExecutable(workingFolder);
        }

        private void PlaceNugetExecutable(string workingFolder)
        {
            try
            {
                string nugetExecutable = Path.Combine(workingFolder, @"Nuget\NuGet.exe");
                string nugetTargets = Path.Combine(workingFolder, @"Nuget\NuGet.targets");

                this.ExtractFileIfRequired(nugetExecutable, InitialEnvironmentResources.NuGetExecutable);
                this.ExtractFileIfRequired(nugetTargets, InitialEnvironmentResources.NuGetTargets);
            }
            catch (Exception e)
            {
                throw new AspNetDeployException("Unable to create nuget resources", e);
            }
        }

        private void CreateRootFolder(string workingFolder)
        {
            if (!Directory.Exists(workingFolder))
            {
                try
                {
                    Directory.CreateDirectory(workingFolder);
                    Directory.CreateDirectory(Path.Combine(workingFolder, "Packages"));
                    Directory.CreateDirectory(Path.Combine(workingFolder, "Sources"));
                }
                catch (Exception e)
                {
                    throw new AspNetDeployException("Unable to create working folder:" + workingFolder, e);
                }
            }
        }

        private void ExtractFileIfRequired(string nugetTargets, byte[] nuGetTargets)
        {
            string directoryName = Path.GetDirectoryName(nugetTargets);

            if (directoryName != null)
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }

            if (!File.Exists(nugetTargets))
            {
                File.WriteAllBytes(nugetTargets, nuGetTargets);
            }
        }
    }
}