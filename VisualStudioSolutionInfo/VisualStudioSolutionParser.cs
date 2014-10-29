using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace VisualStudioSolutionInfo
{
    public class VisualStudioSolutionParser
    {
        readonly Regex projectParser = new Regex(@"Project\(""\{(?<type>[A-F0-9-]+)\}""\) = ""(?<name>[^""]+)"", ""(?<file>[^""]+)"", ""\{(?<guid>[A-F0-9-]+)\}""", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public VisualStudioSolution Parse(string fileName)
        {
            VisualStudioSolution solution = new VisualStudioSolution();

            string text = File.ReadAllText(fileName);
            string folder = Path.GetDirectoryName(fileName);

            Match match = projectParser.Match(text);

            while (match.Success)
            {
                if (match.Groups["type"].Value == "2150E333-8FDC-42A3-9474-1A3956D46DE8") // project group
                {
                    match = match.NextMatch();
                    continue;
                }

                VisualStudioProject visualStudioProject = new VisualStudioProject()
                {
                    Guid = Guid.Parse(match.Groups["guid"].Value),
                    Name = match.Groups["name"].Value,
                    ProjectFile = match.Groups["file"].Value,
                    TypeGuid = Guid.Parse(match.Groups["type"].Value),
                };

                string projectFile = Path.Combine(folder, visualStudioProject.ProjectFile);


                XDocument xDocument = XDocument.Load(projectFile);

                XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

                if (xDocument.Descendants(fileNamespace + "UseIISExpress").Any())
                {
                    visualStudioProject.Type = VisualStudioProjectType.Web;
                }
                else if (visualStudioProject.TypeGuid.ToString() == "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC")
                {
                    visualStudioProject.Type = VisualStudioProjectType.ClassLibrary;
                }

                solution.Projects.Add(visualStudioProject);

                match = match.NextMatch();
            }

            return solution;
        }

    }
}