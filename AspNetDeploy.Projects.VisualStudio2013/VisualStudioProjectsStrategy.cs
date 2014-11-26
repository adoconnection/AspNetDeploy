using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;

namespace AspNetDeploy.Projects.VisualStudio2013
{
    public class VisualStudioProjectsStrategy : IProjectsStrategy
    {
        private readonly string sourcesFolder;
        private readonly ISolutionParsersFactory solutionParsersFactory;

        private IList<ParsedProject> parsedProjects;

        public VisualStudioProjectsStrategy(string sourcesFolder, ISolutionParsersFactory solutionParsersFactory)
        {
            this.sourcesFolder = sourcesFolder;
            this.solutionParsersFactory = solutionParsersFactory;
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
            ParsedProject parsedProject = this.parsedProjects.FirstOrDefault( g => g.Project.Guid == guid);
            project.Name = parsedProject.Project.Name;
        }

        public void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid)
        {
            ParsedProject parsedProject = this.parsedProjects.FirstOrDefault( g => g.Project.Guid == guid);

            projectVersion.Name = parsedProject.Project.Name;
            projectVersion.ProjectFile = parsedProject.Project.ProjectFile;
            projectVersion.ProjectType = parsedProject.Project.Type;
            projectVersion.SolutionFile = Path.GetFileName(parsedProject.SolutionFile);
        }

        public void LoadProjects()
        {
            this.parsedProjects = new List<ParsedProject>();

            List<string> solutionFiles = Directory.GetFiles(sourcesFolder, "*.sln", SearchOption.TopDirectoryOnly).ToList();

            foreach (string solutionFile in solutionFiles)
            {
                ISolutionParser solutionParser = this.solutionParsersFactory.Create(SolutionType.VisualStudio);
                IList<ISolutionProject> projects = solutionParser.Parse(solutionFile);

                foreach (ISolutionProject solutionProject in projects)
                {
                    this.parsedProjects.Add(new ParsedProject()
                    {
                        Project = solutionProject,
                        SolutionFile = solutionFile
                    });
                }
            }
        }

        private struct ParsedProject
        {
            public string SolutionFile { get; set; }
            public ISolutionProject Project { get; set; }
        }
    }
}