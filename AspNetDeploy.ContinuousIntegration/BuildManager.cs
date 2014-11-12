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
        private readonly IPathServices pathServices;

        public BuildManager(IBuildServiceFactory buildServiceFactory, IPathServices pathServices)
        {
            this.buildServiceFactory = buildServiceFactory;
            this.pathServices = pathServices;
        }

        public void Build(int sourceControlVersionId, string solutionFileName, Action<int> projectBuildStarted, Action<int, bool> projectBuildComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControlVersion sourceControlVersion = entities.SourceControlVersion.Include("SourceControl").First( scv => scv.Id == sourceControlVersionId);

            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(sourceControlVersion.SourceControl.Id, sourceControlVersion.Id);
            IBuildService buildService = buildServiceFactory.Create(SolutionType.VisualStudio);

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