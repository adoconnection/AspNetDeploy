using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Ionic.Zip;

namespace Packagers.VisualStudioProject
{
    public class WebProjectPackager : VisualStudioProjectPackager
    {
        protected override void PackageProjectContents(ZipFile zipFile, XDocument xDocument, XNamespace vsNamespace, string projectRootFolder)
        {
            List<string> content = xDocument.Descendants(vsNamespace + "Content")
                    .Where(e => e.Attribute("Include") != null)
                    .Select(e => e.Attribute("Include").Value)
                    .ToList();

            foreach (string file in content)
            {
                AddProjectFile(zipFile, projectRootFolder, file);
            }

            XElement outputPath = xDocument.Descendants(vsNamespace + "PropertyGroup")
                .Where(e => e.Attribute("Condition") != null)
                .Where(e => e.Attribute("Condition").Value.Contains("$(Configuration)|$(Platform)' == 'Release|AnyCPU'"))
                .Descendants(vsNamespace + "OutputPath")
                .FirstOrDefault();

            AddProjectDirectory(
                zipFile, 
                projectRootFolder, 
                outputPath != null ? outputPath.Value : "bin");
        }
    }
}