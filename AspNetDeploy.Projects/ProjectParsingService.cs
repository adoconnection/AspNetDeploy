using System.Collections.Generic;
using AspNetDeploy.Contracts;
using AspNetDeploy.Projects.Contracts;
using AspNetDeploy.Projects.VisualStudio2013;
using AspNetDeploy.Projects.Zip;
using Projects.Gulp;

namespace AspNetDeploy.Projects
{
    public class ProjectParsingService : IProjectParsingService
    {
        private readonly IPathServices pathServices;

        public ProjectParsingService(IPathServices pathServices)
        {
            this.pathServices = pathServices;
        }

        public void UpdateProjects(int sourceControlVersionId)
        {
            ProjectParsingDataContext dataContext = CreateDataContext(sourceControlVersionId);
            IList<IProjectParser> strategies = this.CreateStrategies(dataContext);

            ProjectParsingManager projectParsingManager = new ProjectParsingManager(strategies, dataContext);
            
            projectParsingManager.Execute();
        }

        private IList<IProjectParser> CreateStrategies(ProjectParsingDataContext dataContext)
        {
            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(dataContext.SourceControlVersion.SourceControlId, dataContext.SourceControlVersion.Id);

            IList<IProjectParser> strategies = new List<IProjectParser>()
            {
                new VisualStudioProjectParser(sourcesFolder),
                new ZipFilesParser(sourcesFolder),
                new GulpParser(sourcesFolder)
            };

            return strategies;
        }

        private static ProjectParsingDataContext CreateDataContext(int sourceControlVersionId)
        {
            ProjectParsingDataContext dataContext = new ProjectParsingDataContext();
            dataContext.Initialize(sourceControlVersionId);

            return dataContext;
        }
    }
}