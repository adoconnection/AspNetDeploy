using System.Collections.Generic;
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

            AddProjectDirectory(
                zipFile,
                projectRootFolder,
                outputPath.Value,
                "\\");
        }
    }
}