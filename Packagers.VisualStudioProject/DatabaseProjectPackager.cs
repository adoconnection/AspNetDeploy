using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ionic.Zip;

namespace Packagers.VisualStudioProject
{
    public class DatabaseProjectPackager : VisualStudioProjectPackager
    {
        protected override void PackageProjectContents(ZipFile zipFile, XDocument xDocument, XNamespace vsNamespace, string projectRootFolder)
        {
            XElement outputPath = xDocument.Descendants(vsNamespace + "PropertyGroup")
                .Where(e => e.Attribute("Condition") != null)
                .Where(e => e.Attribute("Condition").Value.Contains("$(Configuration)|$(Platform)' == 'Release|AnyCPU'"))
                .Descendants(vsNamespace + "OutputPath")
                .FirstOrDefault();

            List<string> dacpacFiles = Directory.GetFiles(Path.Combine(projectRootFolder, outputPath.Value), "*.dacpac").ToList();

            if (dacpacFiles.Any())
            {
                AddProjectFile(
                    zipFile,
                    projectRootFolder,
                    dacpacFiles.First(),
                    "\\",
                    "database.dacpac");
            }
        }
    }
}