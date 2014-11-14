using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AspNetDeploy.Contracts;
using Ionic.Zip;
using Ionic.Zlib;

namespace Packagers.VisualStudioProject
{
    public class VisualStudioProjectPackager : IProjectPackager
    {
        public void Package(string projectPath, string packageFile)
        {
            XDocument xDocument = XDocument.Load(projectPath);
            XNamespace vsNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

            string projectRootFolder = Path.GetDirectoryName(projectPath);

            using (ZipFile zipFile = new ZipFile(Encoding.UTF8))
            {
                zipFile.CompressionLevel = CompressionLevel.BestCompression;

                List<string> content = xDocument.Descendants(vsNamespace + "Content")
                    .Where(e => e.Attribute("Include") != null)
                    .Select(e => e.Attribute("Include").Value)
                    .ToList();

                foreach (string file in content)
                {
                    AddProjectFile(zipFile, projectRootFolder, file);
                }

                AddProjectDirectory(zipFile, projectRootFolder, "bin");

                zipFile.Save(packageFile);
            }
        }

        private static void AddProjectDirectory(ZipFile zipFile, string projectRootFolder, string directory)
        {
            string directoryPath = Path.Combine(projectRootFolder, directory);

            if (Directory.Exists(directoryPath))
            {
                zipFile.AddDirectory(directoryPath, directory);
            }
        }

        private static void AddProjectFile(ZipFile zipFile, string projectRootFolder, string file)
        {
            string filePath = Path.Combine(projectRootFolder, file);
            string directoryPathInArchive = Path.GetDirectoryName(file);

            if (File.Exists(filePath))
            {
                zipFile.AddFile(filePath, directoryPathInArchive);
            }
        }
    }
}
