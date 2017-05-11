using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;
using Guids;

namespace AspNetDeploy.Projects.Zip
{
    public class ZipFilesParser : IProjectParser
    {
        private readonly string sourcesFolder;
        private IList<ZipProject> parsedProjects;

        public ZipFilesParser(string sourcesFolder)
        {
            this.sourcesFolder = sourcesFolder;
        }

        public void LoadProjects()
        {
            this.parsedProjects = Directory.GetFiles(this.sourcesFolder, "*.zip", SearchOption.TopDirectoryOnly)
                .Select(f => new ZipProject
                {
                    FilePath = f,
                    FileName = Path.GetFileName(f),
                    Guid = GuidUtility.Create(GuidUtility.UrlNamespace, f.Substring(this.sourcesFolder.Length).TrimStart('\\'))
                })
                .ToList();
        }

        public void UpdateProject(Project project, Guid guid)
        {
            ZipProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);
            project.Name = parsedProject.FileName;
        }

        public void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid)
        {
            ZipProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);

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
    }
}