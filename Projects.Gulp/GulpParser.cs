using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.Projects.Contracts;
using Guids;

namespace Projects.Gulp
{
    public class GulpParser : IProjectParser
    {
        private readonly string sourcesFolder;
        private IList<GulpProject> parsedProjects;

        public GulpParser(string sourcesFolder)
        {
            this.sourcesFolder = sourcesFolder;
        }

        public void LoadProjects()
        {
            this.parsedProjects = Directory.GetFiles(this.sourcesFolder, "*.gulpfile.js", SearchOption.TopDirectoryOnly)
                    .Select(f => new GulpProject
                    {
                        FilePath = f,
                        FileName = Path.GetFileName(f),
                        Guid = GuidUtility.Create(GuidUtility.UrlNamespace, f.Substring(this.sourcesFolder.Length).TrimStart('\\'))
                    })
                    .ToList();
        }

        public IList<Guid> ListProjectGuids()
        {
            return this.parsedProjects.Select(p => p.Guid).ToList();
        }

        public bool IsExists(Guid guid)
        {
            return this.parsedProjects.Any(g => g.Guid == guid);
        }

        public void UpdateProject(Project project, Guid guid)
        {
            GulpProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);
            project.Name = parsedProject.FileName;
        }

        public void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid)
        {
            GulpProject parsedProject = this.parsedProjects.FirstOrDefault(g => g.Guid == guid);

            projectVersion.Name = parsedProject.FileName;
            projectVersion.ProjectFile = parsedProject.FileName;
            projectVersion.ProjectType = ProjectType.GulpFile;
            projectVersion.SolutionFile = string.Empty;
        }
    }
}