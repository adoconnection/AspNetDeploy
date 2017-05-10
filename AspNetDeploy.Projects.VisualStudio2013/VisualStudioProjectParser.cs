using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;
using AspNetDeploy.SolutionParsers.VisualStudio;

namespace AspNetDeploy.Projects.VisualStudio2013
{
    public class VisualStudioProjectParser : IProjectParser
    {
        private readonly string sourcesFolder;

        private IList<VsProject> parsedProjects;

        public VisualStudioProjectParser(string sourcesFolder)
        {
            this.sourcesFolder = sourcesFolder;
        }

        public IList<Guid> ListProjectGuids()
        {
            return this.parsedProjects.Select(p => p.Project.Guid).ToList();
        }

        public bool IsExists(Guid guid)
        {
            return this.parsedProjects.Any( g => g.Project.Guid == guid );
        }

        public void UpdateProject(Project project, Guid guid)
        {
            VsProject vsProject = this.parsedProjects.FirstOrDefault( g => g.Project.Guid == guid);
            project.Name = vsProject.Project.Name;
        }

        public void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid)
        {
            VsProject vsProject = this.parsedProjects.FirstOrDefault( g => g.Project.Guid == guid);

            projectVersion.Name = vsProject.Project.Name;
            projectVersion.ProjectFile = vsProject.Project.ProjectFile;

            ProjectType projectType = ProjectType.Undefined; // will be removed when ProjectType will be moved to database as a table

            if (vsProject.Project.Type.HasFlag(VsProjectType.ClassLibrary))
            {
                projectType |= ProjectType.ClassLibrary;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Console))
            {
                projectType |= ProjectType.Console;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Database))
            {
                projectType |= ProjectType.Database;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Deployment))
            {
                projectType |= ProjectType.Deployment;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Service))
            {
                projectType |= ProjectType.Service;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Test))
            {
                projectType |= ProjectType.Test;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.Web))
            {
                projectType |= ProjectType.Web;
            }

            if (vsProject.Project.Type.HasFlag(VsProjectType.WindowsApplication))
            {
                projectType |= ProjectType.WindowsApplication;
            }

            projectVersion.ProjectType = projectType;

            projectVersion.SolutionFile = Path.GetFileName(vsProject.SolutionFile);
        }

        public void LoadProjects()
        {
            this.parsedProjects = new List<VsProject>();

            List<string> solutionFiles = Directory.GetFiles(sourcesFolder, "*.sln", SearchOption.TopDirectoryOnly).ToList();

            foreach (string solutionFile in solutionFiles)
            {
                VisualStudio2013SolutionParser solutionParser = new VisualStudio2013SolutionParser();
                IList<VisualStudioSolutionProject> projects = solutionParser.Parse(solutionFile);

                foreach (VisualStudioSolutionProject solutionProject in projects)
                {
                    this.parsedProjects.Add(new VsProject()
                    {
                        Project = solutionProject,
                        SolutionFile = solutionFile
                    });
                }
            }
        }
    }
}