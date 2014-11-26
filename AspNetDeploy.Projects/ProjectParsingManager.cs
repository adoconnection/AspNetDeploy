using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;

namespace AspNetDeploy.Projects
{
    public class ProjectParsingManager
    {
        private readonly IList<IProjectsStrategy> strategies;
        private readonly ProjectParsingDataContext dataContext;

        public ProjectParsingManager(IList<IProjectsStrategy> strategies, ProjectParsingDataContext dataContext)
        {
            this.strategies = strategies;
            this.dataContext = dataContext;
        }

        public void Execute()
        {
            this.Initialize();
            this.UpdateOrDeleteProjects();
            this.CreateNewProjects();
            this.dataContext.SaveChanges();
        }

        private void CreateNewProjects()
        {
            IList<Project> existingProjects = this.dataContext.ListExistingProjects();

            foreach (IProjectsStrategy strategy in this.strategies)
            {
                IList<Guid> guids = strategy.ListProjectGuids();

                foreach (Guid guid in guids)
                {
                    if (existingProjects.Any(p => p.Guid == guid))
                    {
                        continue;
                    }

                    Project project = this.dataContext.CreateProject();
                    project.Guid = guid;
                    strategy.UpdateProject(project, guid);

                    ProjectVersion projectVersion = this.dataContext.CreateProjectVersion(project);
                    strategy.UpdateProjectVersion(projectVersion, guid);
                    projectVersion.IsDeleted = false;
                }
            }
        }

        private void UpdateOrDeleteProjects()
        {
            foreach (Project existingProject in this.dataContext.ListExistingProjects())
            {
                ProjectVersion projectVersion = this.dataContext.GetProjectVersion(existingProject.Id);
                IProjectsStrategy strategy = this.strategies.FirstOrDefault(s => s.IsExists(existingProject.Guid));

                if (strategy == null)
                {
                    if (projectVersion != null)
                    {
                        projectVersion.IsDeleted = true;
                    }
                }
                else
                {
                    if (projectVersion == null)
                    {
                        projectVersion = this.dataContext.CreateProjectVersion(existingProject);
                    }

                    strategy.UpdateProjectVersion(projectVersion, existingProject.Guid);
                    projectVersion.IsDeleted = false;
                }
            }
        }

        private void Initialize()
        {
            foreach (IProjectsStrategy strategy in this.strategies)
            {
                strategy.LoadProjects();
            }
        }
    }
}