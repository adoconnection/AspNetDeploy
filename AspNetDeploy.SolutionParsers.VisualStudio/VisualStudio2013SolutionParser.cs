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
        readonly Regex guidParser = new Regex(@"{(?<guid>[^}]+)}", RegexOptions.Singleline | RegexOptions.IgnoreCase);

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
            XElement projectTypeGuids = xDocument.Descendants(fileNamespace + "ProjectTypeGuids").FirstOrDefault();

            if (projectTypeGuids != null)
            {
                Match match = guidParser.Match(projectTypeGuids.Value);

                while (match.Success)
                {
                    visualStudioProject.Type |= this.GetTypeByGuid(match.Groups["guid"].Value);
                    match = match.NextMatch();
                }
            }

            string typeGuid = visualStudioProject.TypeGuid.ToString();

            XElement outputTypeElement = xDocument.Descendants(fileNamespace + "OutputType").FirstOrDefault();

            if (outputTypeElement != null && outputTypeElement.Value == "Exe")
            {
                visualStudioProject.Type |= ProjectType.Console;
            }
            else if (outputTypeElement != null && outputTypeElement.Value == "WinExe")
            {
                visualStudioProject.Type |= ProjectType.WindowsApplication;
            }
            else if (outputTypeElement != null && outputTypeElement.Value == "Database")
            {
                visualStudioProject.Type |= ProjectType.Database;
            }
            else if (xDocument.Descendants(fileNamespace + "UseIISExpress").Any())
            {
                visualStudioProject.Type |= ProjectType.Web;

                switch (typeGuid.ToUpper())
                {
                    case "E3E379DF-F4C6-4180-9B81-6769533ABE47":
                        visualStudioProject.MvcVersion = 4;
                        break;
                    case "E53F8FEA-EAE0-44A6-8774-FFD645390401":
                        visualStudioProject.MvcVersion = 3;
                        break;
                    case "F85E285D-A4E0-4152-9332-AB1D724D3325":
                        visualStudioProject.MvcVersion = 2;
                        break;
                    case "603C0E0B-DB56-11DC-BE95-000D561079B0":
                        visualStudioProject.MvcVersion = 1;
                        break;
                    default:
                        break;
                }
            }
        }

        private ProjectType GetTypeByGuid(string typeGuid)
        {
            switch (typeGuid.ToUpper())
            {
                case "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC":
                    return ProjectType.ClassLibrary;

                case "A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124":
                    return ProjectType.Database;

                case "3AC096D0-A1C2-E12C-1390-A8335801FDAB":
                    return ProjectType.Test;

                case "349C5851-65DF-11DA-9384-00065B846F21":
                    return ProjectType.Web;

                case "00D1A9C2-B5F0-4AF3-8072-F6C62B433612":
                    return ProjectType.Database;

                default:
                    return ProjectType.Undefined;
            }
        }
    }
}
