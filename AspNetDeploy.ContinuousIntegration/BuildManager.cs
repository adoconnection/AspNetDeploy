using System;
using System.Globalization;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.ContinuousIntegration
{
    public class BuildManager
    {
        private readonly IBuildServiceFactory buildServiceFactory;

        public BuildManager(IBuildServiceFactory buildServiceFactory)
        {
            this.buildServiceFactory = buildServiceFactory;
        }

        public void Build(int sourceControlId, string solutionFileName, Action<int> projectBuildStarted, Action<int, bool> projectBuildComplete)
        {
            string sourcesFolder = string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\trunk", sourceControlId);
            IBuildService buildService = buildServiceFactory.Create(SolutionType.VisualStudio);

            AspNetDeployEntities entities = new AspNetDeployEntities();

            buildService.Build(
                Path.Combine(sourcesFolder, solutionFileName),
                projectFileName =>
                {
                    Project project = entities.Project
                        .Where(p => p.SourceControlId == sourceControlId)
                        .ToList()
                        .FirstOrDefault(p => Path.Combine(sourcesFolder, p.ProjectFile).ToLowerInvariant() == projectFileName.ToLowerInvariant());

                    if (project != null)
                    {
                        projectBuildStarted(project.Id);
                    }
                },
                (projectFileName, success) =>
                {
                    Project project = entities.Project
                        .Where(p => p.SourceControlId == sourceControlId)
                        .ToList()
                        .FirstOrDefault(p => Path.Combine(sourcesFolder, p.ProjectFile).ToLowerInvariant() == projectFileName.ToLowerInvariant());

                    if (project != null)
                    {
                        projectBuildComplete(project.Id, success);
                    }
                });
        }
    }
}