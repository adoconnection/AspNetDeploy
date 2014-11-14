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
        private readonly IPathServices pathServices;

        public SourceControlManager(ISourceControlRepositoryFactory sourceControlRepositoryFactory, ISolutionParsersFactory solutionParsersFactory, IPathServices pathServices)
        {
            this.sourceControlRepositoryFactory = sourceControlRepositoryFactory;
            this.solutionParsersFactory = solutionParsersFactory;
            this.pathServices = pathServices;
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

            IList<string> solutionFiles = this.ListSolutionFiles(sourcesFolder);
            this.UpdateProjects(solutionFiles, sourceControlVersion, entities);

            entities.SaveChanges();

            return new UpdateAndParseResult
            {
                HasChanges = true,
                Projects = entities.Project.Where(p => p.SourceControlId == sourceControlVersion.Id).Select( p => p.Id).ToList()
            };
        }

        private void UpdateProjects(IEnumerable<string> solutionFiles, SourceControlVersion sourceControlVersion, AspNetDeployEntities entities)
        {
            //bool isHead = sourceControlVersion.GetBoolProperty("IsHeadVersion");

            IList<Project> existingProjects = entities.Project
                .Include("ProjectVersions")
                .Where(p => p.SourceControlId == sourceControlVersion.SourceControlId).ToList();

            List<ProjectVersion> existingProjectVersions = existingProjects
                .SelectMany(p => p.ProjectVersions)
                .Where(pv => pv.SourceControlVersionId == sourceControlVersion.Id)
                .ToList();

            foreach (string solutionFile in solutionFiles)
            {
                ISolutionParser solutionParser = this.solutionParsersFactory.Create(SolutionType.VisualStudio);
                IList<ISolutionProject> parsedProjects = solutionParser.Parse(solutionFile);

                foreach (Project project in existingProjects)
                {
                    ProjectVersion projectVersion = existingProjectVersions.FirstOrDefault(pv => pv.ProjectId == project.Id);
                    ISolutionProject parsedProject = parsedProjects.FirstOrDefault(sp => sp.Guid == project.Guid);

                    if (parsedProject == null)
                    {
                        if (projectVersion != null)
                        {
                            projectVersion.IsDeleted = true;
                        }
                        else
                        {
                            //continue;
                        }
                    }
                    else
                    {
                        if (projectVersion == null)
                        {
                            projectVersion = new ProjectVersion();
                            projectVersion.Project = project;
                            projectVersion.SourceControlVersion = sourceControlVersion;
                            entities.ProjectVersion.Add(projectVersion);
                        }

                        projectVersion.Name = parsedProject.Name;
                        projectVersion.ProjectFile = parsedProject.ProjectFile;
                        projectVersion.ProjectType = parsedProject.Type;
                        projectVersion.IsDeleted = false;
                        projectVersion.SolutionFile = Path.GetFileName(solutionFile);
                    }
                }

                foreach (ISolutionProject parsedProject in parsedProjects)
                {
                    if (existingProjects.Any(p => p.Guid == parsedProject.Guid))
                    {
                        continue;
                    }

                    Project project = new Project();
                    project.SourceControlId = sourceControlVersion.SourceControl.Id;
                    entities.Project.Add(project);

                    project.Guid = parsedProject.Guid;
                    project.Name = parsedProject.Name;

                    ProjectVersion projectVersion = new ProjectVersion();
                    projectVersion.Project = project;
                    projectVersion.SourceControlVersion = sourceControlVersion;
                    entities.ProjectVersion.Add(projectVersion);

                    projectVersion.Name = parsedProject.Name;
                    projectVersion.ProjectFile = parsedProject.ProjectFile;
                    projectVersion.ProjectType = parsedProject.Type;
                    projectVersion.IsDeleted = false;
                    projectVersion.SolutionFile = Path.GetFileName(solutionFile);
                }
            }
        }

        private IList<string> ListSolutionFiles(string sourcesFolder)
        {
            return Directory.GetFiles(sourcesFolder, "*.sln", SearchOption.TopDirectoryOnly).ToList();
        }

        private LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string sourcesFolder)
        {
            ISourceControlRepository repository = this.sourceControlRepositoryFactory.Create(sourceControlVersion.SourceControl.Type);
            return repository.LoadSources(sourceControlVersion, sourcesFolder);
        }
    }
}