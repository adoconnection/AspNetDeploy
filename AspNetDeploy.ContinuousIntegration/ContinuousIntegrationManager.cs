using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.ContinuousIntegration
{
    public class ContinuousIntegrationManager
    {
        private readonly IBuildServiceFactory buildServiceFactory;
        private readonly ISolutionParsersFactory solutionParsersFactory;
        private readonly ISourceControlRepositoryFactory sourceControlRepositoryFactory;

        public ContinuousIntegrationManager(IBuildServiceFactory buildServiceFactory, ISolutionParsersFactory solutionParsersFactory, ISourceControlRepositoryFactory sourceControlRepositoryFactory)
        {
            this.buildServiceFactory = buildServiceFactory;
            this.solutionParsersFactory = solutionParsersFactory;
            this.sourceControlRepositoryFactory = sourceControlRepositoryFactory;
        }

        public void Run(SourceControl sourceControl, IContinuousIntegrationLogger logger)
        {
            string sourcesFolder = string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\trunk", sourceControl.Id);

            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControl.Type);
            LoadSourcesResult loadSourcesResult = repository.LoadSources(sourceControl, "trunk", sourcesFolder);

            IList<string> solutionFiles = Directory.GetFiles(sourcesFolder, "*.sln", SearchOption.TopDirectoryOnly).ToList();

            AspNetDeployEntities entities = new AspNetDeployEntities();
            IList<Project> existingProjects = entities.Project.Where( p => p.SourceControlId == sourceControl.Id).ToList();

            foreach (string solutionFile in solutionFiles)
            {
                ISolutionParser solutionParser = solutionParsersFactory.Create(SolutionType.VisualStudio);
                IList<ISolutionProject> parsedProjects = solutionParser.Parse(solutionFile);

                foreach (Project project in existingProjects)
                {
                    ISolutionProject parsedProject = parsedProjects.FirstOrDefault( sp => sp.Guid ==  project.Guid);

                    if (parsedProject == null)
                    {
                        project.IsDeleted = true;
                    }
                    else
                    {
                        project.IsDeleted = false;
                        project.SolutionFile = Path.GetFileName(solutionFile);
                    }
                }

                foreach (ISolutionProject parsedProject in parsedProjects)
                {
                    if (existingProjects.All(p => p.Guid != parsedProject.Guid))
                    {
                        Project project = new Project();
                        project.SourceControlId = sourceControl.Id;
                        project.Guid = parsedProject.Guid;
                        project.Name = parsedProject.Name;
                        project.ProjectType = parsedProject.Type;
                        project.IsDeleted = false;
                        project.SolutionFile = Path.GetFileName(solutionFile);
                        entities.Project.Add(project);
                    }
                }
            }

            entities.SaveChanges();

            /*IBuildService buildService = this.buildServiceFactory.Create(SolutionType.VisualStudio);

            foreach (string solutionFile in solutionFiles)
            {
                buildService.Build(solutionFile);
            }*/

        }
    }
}
