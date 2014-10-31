using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.SolutionParsers.VisualStudio
{
    public class VisualStudio2013SolutionParser : ISolutionParser
    {
        readonly Regex projectParser = new Regex(@"Project\(""\{(?<type>[A-F0-9-]+)\}""\) = ""(?<name>[^""]+)"", ""(?<file>[^""]+)"", ""\{(?<guid>[A-F0-9-]+)\}""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public IList<ISolutionProject> Parse(string solutionFilePath)
        {
            string text = File.ReadAllText(solutionFilePath);
            string folder = Path.GetDirectoryName(solutionFilePath);

            IList<ISolutionProject> result = new List<ISolutionProject>();

            Match match = projectParser.Match(text);

            while (match.Success)
            {
                if (match.Groups["type"].Value == "2150E333-8FDC-42A3-9474-1A3956D46DE8") // project group
                {
                    match = match.NextMatch();
                    continue;
                }

                VisualStudioSolutionProject visualStudioProject = new VisualStudioSolutionProject()
                {
                    Guid = Guid.Parse(match.Groups["guid"].Value),
                    Name = match.Groups["name"].Value,
                    ProjectFile = match.Groups["file"].Value,
                    TypeGuid = Guid.Parse(match.Groups["type"].Value),
                };

                string projectFile = Path.Combine(folder, visualStudioProject.ProjectFile);
                this.ProcessCSProjFile(projectFile, visualStudioProject);

                result.Add(visualStudioProject);

                match = match.NextMatch();
            }

            return result;
        }

        private void ProcessCSProjFile(string projectFile, VisualStudioSolutionProject visualStudioProject)
        {
            XDocument xDocument = XDocument.Load(projectFile);

            XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

            this.DetermineProjectType(visualStudioProject, xDocument, fileNamespace);
            this.DetermineProjectCLR(visualStudioProject, xDocument, fileNamespace);
        }

        private void DetermineProjectCLR(VisualStudioSolutionProject visualStudioProject, XDocument xDocument, XNamespace fileNamespace)
        {
            visualStudioProject.TargetFrameworkVersion = xDocument.Descendants(fileNamespace + "TargetFrameworkVersion").Select(n => n.Value).FirstOrDefault();
            visualStudioProject.ContentFiles = xDocument.Descendants(fileNamespace + "Content").Select(n => n.Attribute("Include").Value).ToList();
            visualStudioProject.ReferenceLibraries = xDocument
                .Descendants(fileNamespace + "Content")
                .SelectMany(n => n.Descendants("HintPath"))
                .Select( n => n.Value)
                .ToList();
        }

        private void DetermineProjectType(VisualStudioSolutionProject visualStudioProject, XDocument xDocument, XNamespace fileNamespace)
        {
            if (xDocument.Descendants(fileNamespace + "UseIISExpress").Any())
            {
                visualStudioProject.Type = ProjectType.Web;
            }
            else if (visualStudioProject.TypeGuid.ToString() == "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC")
            {
                visualStudioProject.Type = ProjectType.ClassLibrary;
            }
        }
    }
}
