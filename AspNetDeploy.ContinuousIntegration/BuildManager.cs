using System;
using System.Globalization;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;

namespace AspNetDeploy.ContinuousIntegration
{
    public class BuildManager
    {
        private readonly IBuildServiceFactory buildServiceFactory;
        private readonly IPathServices pathServices;
        private readonly ILoggingService loggingService;

        public BuildManager(IBuildServiceFactory buildServiceFactory, IPathServices pathServices, ILoggingService loggingService)
        {
            this.buildServiceFactory = buildServiceFactory;
            this.pathServices = pathServices;
            this.loggingService = loggingService;
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
                (projectFileName, success, message) =>
                {
                    ProjectVersion projectVersion = entities.ProjectVersion
                        .Where(p => p.SourceControlVersionId == sourceControlVersionId)
                        .ToList()
                        .FirstOrDefault(p => Path.Combine(sourcesFolder, p.ProjectFile).ToLowerInvariant() == projectFileName.ToLowerInvariant());

                    if (projectVersion != null)
                    {
                        projectBuildComplete(projectVersion.Id, success);
                    }
                },
                (projectFile, file, code, lineNumber, columnNumber, message) =>
                {
                    AspNetDeployException exception = new AspNetDeployException(
                        string.Format(
                            "File: {0}. code: {1}, lineNumber: {2}, columnNumber: {3}, message: {4}", 
                            file, 
                            code,
                            lineNumber, 
                            columnNumber,
                            message));

                    this.loggingService.Log(new AspNetDeployException("Project build failed: " + projectFile, exception), null);
                });
        }
    }
}