using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.ContinuousIntegration
{
    public class SourceControlManager
    {
        private readonly ISourceControlRepositoryFactory sourceControlRepositoryFactory;
        private readonly ISolutionParsersFactory solutionParsersFactory;

        public SourceControlManager(ISourceControlRepositoryFactory sourceControlRepositoryFactory, ISolutionParsersFactory solutionParsersFactory)
        {
            this.sourceControlRepositoryFactory = sourceControlRepositoryFactory;
            this.solutionParsersFactory = solutionParsersFactory;
        }

        public UpdateAndParseResult UpdateAndParse(int sourceControlId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            SourceControl sourceControl = entities.SourceControl.Include("Properties").First(sc => sc.Id == sourceControlId);

            string sourcesFolder = string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\trunk", sourceControl.Id);
            LoadSourcesResult loadSourcesResult = this.LoadSources(sourceControl, sourcesFolder);

            if (loadSourcesResult.RevisionId == sourceControl.GetStringProperty("Revision"))
            {
                return new UpdateAndParseResult();
            }

            sourceControl.SetStringProperty("Revision", loadSourcesResult.RevisionId);
            entities.SaveChanges();

            IList<string> solutionFiles = this.ListSolutionFiles(sourcesFolder);
            this.UpdateProjects(solutionFiles, sourceControl, entities);

            entities.SaveChanges();

            return new UpdateAndParseResult
            {
                HasChanges = true,
                Projects = entities.Project.Where(p => p.SourceControlId == sourceControl.Id).Select( p => p.Id).ToList()
            };
        }

        private void UpdateProjects(IEnumerable<string> solutionFiles, SourceControl sourceControl, AspNetDeployEntities entities)
        {
            IList<Project> existingProjects = entities.Project.Where(p => p.SourceControlId == sourceControl.Id).ToList();

            foreach (string solutionFile in solutionFiles)
            {
                ISolutionParser solutionParser = this.solutionParsersFactory.Create(SolutionType.VisualStudio);
                IList<ISolutionProject> parsedProjects = solutionParser.Parse(solutionFile);

                foreach (Project project in existingProjects)
                {
                    ISolutionProject parsedProject = parsedProjects.FirstOrDefault(sp => sp.Guid == project.Guid);

                    if (parsedProject == null)
                    {
                        project.IsDeleted = true;
                    }
                    else
                    {
                        project.IsDeleted = false;
                        project.ProjectFile = parsedProject.ProjectFile;
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
                        project.ProjectFile = parsedProject.ProjectFile;
                        project.ProjectType = parsedProject.Type;
                        project.IsDeleted = false;
                        project.SolutionFile = Path.GetFileName(solutionFile);
                        entities.Project.Add(project);
                    }
                }
            }
        }

        private IList<string> ListSolutionFiles(string sourcesFolder)
        {
            return Directory.GetFiles(sourcesFolder, "*.sln", SearchOption.TopDirectoryOnly).ToList();
        }

        private LoadSourcesResult LoadSources(SourceControl sourceControl, string sourcesFolder)
        {
            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControl.Type);
            return repository.LoadSources(sourceControl, "trunk", sourcesFolder);
        }
    }
}