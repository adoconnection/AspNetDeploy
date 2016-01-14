using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using Guids;

namespace AspNetDeploy.ContinuousIntegration
{
    public class SourceControlManager
    {
        private readonly ISourceControlRepositoryFactory sourceControlRepositoryFactory;
        private readonly IPathServices pathServices;
        private readonly IProjectParsingService projectParsingService;

        public SourceControlManager(ISourceControlRepositoryFactory sourceControlRepositoryFactory, IPathServices pathServices, IProjectParsingService projectParsingService)
        {
            this.sourceControlRepositoryFactory = sourceControlRepositoryFactory;
            this.pathServices = pathServices;
            this.projectParsingService = projectParsingService;
        }

        public UpdateAndParseResult UpdateAndParse(int sourceControlVersionId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            SourceControlVersion sourceControlVersion = entities.SourceControlVersion
                .Include("Properties")
                .Include("SourceControl.Properties")
                .First(svc => svc.Id == sourceControlVersionId);

            SourceControl sourceControl = sourceControlVersion.SourceControl;

            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(sourceControl.Id, sourceControlVersion.Id);
            LoadSourcesResult loadSourcesResult = this.LoadSources(sourceControlVersion, sourcesFolder);

            if (loadSourcesResult.RevisionId == sourceControlVersion.GetStringProperty("Revision"))
            {
                return new UpdateAndParseResult();
            }

            sourceControlVersion.SetStringProperty("Revision", loadSourcesResult.RevisionId);
            entities.SaveChanges();

            this.projectParsingService.UpdateProjects(sourceControlVersionId);

            return new UpdateAndParseResult
            {
                HasChanges = true,
                Projects = entities.Project.Where(p => p.SourceControlId == sourceControlVersion.Id).Select(p => p.Id).ToList()
            };
        }

        public TestSourceResult TestConnection(SourceControlVersion sourceControlVersion)
        {
            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControlVersion.SourceControl.Type);
            return repository.TestConnection(sourceControlVersion);

            /*string sourcesFolder = this.pathServices.GetSourceControlVersionPath(sourceControlVersion.SourceControl.Id, sourceControlVersion.Id);
            string revisionId = null;
            string exceptionMessage = null;

            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControlVersion.SourceControl.Type);
            repository.Archive(sourceControlVersion, sourcesFolder);

            Task.Run(() =>
            {
                try
                {
                    LoadSourcesResult loadSourcesResult = this.LoadSources(sourceControlVersion, sourcesFolder);
                    revisionId = loadSourcesResult.RevisionId;
                }
                catch (AspNetDeployException e)
                {
                    exceptionMessage = e.InnerException.Message;
                }
            }).Wait(1000);

            


            return new TestSourceResult()
            {
                IsSuccess = revisionId != null,
                ErrorMessage = exceptionMessage
            };*/

            return new TestSourceResult
            {
                IsSuccess = true
            };
        }

        public ArhiveResult Archive(int sourceControlVersionId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControlVersion sourceControlVersion = entities.SourceControlVersion
                .Include("Properties")
                .First(sc => sc.Id == sourceControlVersionId);

            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControlVersion.SourceControl.Type);
            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(sourceControlVersion.SourceControlId, sourceControlVersion.Id);

            repository.Archive(sourceControlVersion, sourcesFolder);

            sourceControlVersion.ArchiveState = SourceControlVersionArchiveState.Archived;
            entities.SaveChanges();

            return new ArhiveResult() { IsSuccess = true };
        }

        private LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string sourcesFolder)
        {
            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControlVersion.SourceControl.Type);
            return repository.LoadSources(sourceControlVersion, sourcesFolder);
        }
    }
}