using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.Projects
{
    public class ProjectParsingDataContext
    {
        private IList<Project> existingProjects;
        private List<ProjectVersion> existingProjectVersions;
        private SourceControlVersion sourceControlVersion;
        private AspNetDeployEntities entities;

        public SourceControlVersion SourceControlVersion
        {
            get
            {
                return this.sourceControlVersion;
            }
        }

        public void Initialize(int sourceControlVersionId)
        {
            this.entities = new AspNetDeployEntities();

            this.sourceControlVersion = this.entities.SourceControlVersion
                .Include("Properties")
                .Include("SourceControl.Properties")
                .First(svc => svc.Id == sourceControlVersionId);

            this.existingProjects = this.entities.Project
                .Include("ProjectVersions")
                .Where(p => p.SourceControlId == sourceControlVersion.SourceControlId)
                .ToList();

            this.existingProjectVersions = existingProjects
                .SelectMany(p => p.ProjectVersions)
                .Where(pv => pv.SourceControlVersionId == sourceControlVersion.Id)
                .ToList();
        }

        public Project CreateProject()
        {
            Project project = new Project();
            project.SourceControlId = sourceControlVersion.SourceControlId;
            
            entities.Project.Add(project);

            return project;
        }

        public ProjectVersion CreateProjectVersion(Project project)
        {
            ProjectVersion projectVersion = new ProjectVersion();

            projectVersion.Project = project;
            projectVersion.SourceControlVersion = sourceControlVersion;
            entities.ProjectVersion.Add(projectVersion);

            return projectVersion;
        }

        public IList<Project> ListExistingProjects()
        {
            return this.existingProjects;
        }

        public ProjectVersion GetProjectVersion(int projectId)
        {
            return existingProjectVersions.FirstOrDefault(pv => pv.ProjectId == projectId);
        }

        public void SaveChanges()
        {
            this.entities.SaveChanges();
        }
    }
}