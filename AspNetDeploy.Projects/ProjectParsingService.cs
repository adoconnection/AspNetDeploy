using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;
using AspNetDeploy.Projects.VisualStudio2013;
using AspNetDeploy.Projects.Zip;
using ObjectFactory;

namespace AspNetDeploy.Projects
{
    public class ProjectParsingService : IProjectParsingService
    {
        private readonly ISolutionParsersFactory solutionParsersFactory;
        private readonly IPathServices pathServices;

        public ProjectParsingService(ISolutionParsersFactory solutionParsersFactory, IPathServices pathServices)
        {
            this.solutionParsersFactory = solutionParsersFactory;
            this.pathServices = pathServices;
        }

        public void UpdateProjects(int sourceControlVersionId)
        {
            ProjectParsingDataContext dataContext = CreateDataContext(sourceControlVersionId);
            IList<IProjectsStrategy> strategies = this.CreateStrategies(dataContext);

            ProjectParsingManager projectParsingManager = new ProjectParsingManager(strategies, dataContext);
            
            projectParsingManager.Execute();
        }

        private IList<IProjectsStrategy> CreateStrategies(ProjectParsingDataContext dataContext)
        {
            string sourcesFolder = this.pathServices.GetSourceControlVersionPath(dataContext.SourceControlVersion.SourceControlId, dataContext.SourceControlVersion.Id);

            IList<IProjectsStrategy> strategies = new List<IProjectsStrategy>()
            {
                new VisualStudioProjectsStrategy(sourcesFolder, this.solutionParsersFactory),
                new ZipFilesStrategy(sourcesFolder)
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