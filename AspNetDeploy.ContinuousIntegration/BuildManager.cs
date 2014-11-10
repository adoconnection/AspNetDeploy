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

        public void Build(int sourceControlVersionId, string solutionFileName, Action<int> projectBuildStarted, Action<int, bool> projectBuildComplete)
        {
            string sourcesFolder = string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\trunk", sourceControlVersionId);
            IBuildService buildService = buildServiceFactory.Create(SolutionType.VisualStudio);

            AspNetDeployEntities entities = new AspNetDeployEntities();

            buildService.Build(
                Path.Combine(sourcesFolder, solutionFileName),
                projectFileName =>
                {
                    ProjectVersion projectVersion = entities.ProjectVersion
                        .Where(p => p.SourceControlVersionId == sourceControlVersionId)
                        .ToList()
                        .FirstOrDefault(p => Path.Combine(sourcesFolder, p.ProjectFile).ToLowerInvariant() == projectFileName.ToLowerInvariant());

                    if (projectVersion != null)
                    {
                        projectBuildStarted(projectVersion.Id);
                    }
                },
                (projectFileName, success) =>
                {
                    ProjectVersion projectVersion = entities.ProjectVersion
                        .Where(p => p.SourceControlVersionId == sourceControlVersionId)
                        .ToList()
                        .FirstOrDefault(p => Path.Combine(sourcesFolder, p.ProjectFile).ToLowerInvariant() == projectFileName.ToLowerInvariant());

                    if (projectVersion != null)
                    {
                        projectBuildComplete(projectVersion.Id, success);
                    }
                });
        }
    }
}