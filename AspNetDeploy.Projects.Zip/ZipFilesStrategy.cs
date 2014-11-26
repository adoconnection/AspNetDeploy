using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;
using Guids;

namespace AspNetDeploy.Projects.Zip
{
    public class ZipFilesStrategy : IProjectsStrategy
    {
        private readonly string sourcesFolder;
        private IList<ParsedProject> parsedProjects;

        public ZipFilesStrategy(string sourcesFolder)
        {
            this.sourcesFolder = sourcesFolder;
        }

        public void LoadProjects()
        {
            this.parsedProjects = Directory.GetFiles(this.sourcesFolder, "*.zip", SearchOption.TopDirectoryOnly)
                .Select(f => new ParsedProject
                {
                    FilePath = f,
                    FileName = Path.GetFileName(f),
                    Guid = GuidUtility.Create(GuidUtility.UrlNamespace, f)
                })
                .ToList();
        }

        public void UpdateProject(Project project, Guid guid)
        {
            ParsedProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);
            project.Name = parsedProject.FileName;
        }

        public void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid)
        {
            ParsedProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);

            projectVersion.Name = parsedProject.FileName;
            projectVersion.ProjectFile = parsedProject.FileName;
            projectVersion.ProjectType = ProjectType.ZipArchive;
            projectVersion.SolutionFile = string.Empty;
        }

        public IList<Guid> ListProjectGuids()
        {
            return this.parsedProjects.Select(p => p.Guid).ToList();
        }

        public bool IsExists(Guid guid)
        {
            return this.parsedProjects.Any(g => g.Guid == guid);
        }


        private struct ParsedProject
        {
            public string FilePath { get; set; } 
            public string FileName { get; set; } 
            public Guid Guid { get; set; } 
        }
    }
}