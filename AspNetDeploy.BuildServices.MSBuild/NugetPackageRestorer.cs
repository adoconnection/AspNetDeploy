using System;
using System.IO;
using AspNetDeploy.Contracts;
using Microsoft.Build.Framework;

namespace AspNetDeploy.BuildServices.MSBuild
{
    public class NugetPackageRestorer : ILogger
    {
        private readonly INugetPackageManager nugetPackageManager;
        private readonly string solutionDirectory;

        public NugetPackageRestorer(INugetPackageManager nugetPackageManager, string solutionDirectory)
        {
            this.Verbosity = LoggerVerbosity.Diagnostic;
            this.nugetPackageManager = nugetPackageManager;
            this.solutionDirectory = solutionDirectory;
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.ProjectStarted += this.OnProjectBuildStarted;
        }

        void OnProjectBuildStarted(object sender, ProjectStartedEventArgs e)
        {
            if (e.ProjectFile.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            string packagesConfigPath = Path.Combine(Path.GetDirectoryName(e.ProjectFile), "packages.config");

            if (File.Exists(packagesConfigPath))
            {
                this.nugetPackageManager.RestorePackages(packagesConfigPath, solutionDirectory);
            }
        }

        public void Shutdown()
        {
        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }
}