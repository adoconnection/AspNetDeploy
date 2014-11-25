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
    public abstract class VisualStudioProjectPackager : IProjectPackager
    {
        protected abstract void PackageProjectContents(ZipFile zipFile, XDocument xDocument, XNamespace vsNamespace, string projectRootFolder);

        public void Package(string projectPath, string packageFile)
        {
            XDocument xDocument = XDocument.Load(projectPath);
            XNamespace vsNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

            string projectRootFolder = Path.GetDirectoryName(projectPath);

            using (ZipFile zipFile = new ZipFile(Encoding.UTF8))
            {
                zipFile.CompressionLevel = CompressionLevel.BestCompression;

                this.PackageProjectContents(zipFile, xDocument, vsNamespace, projectRootFolder);

                

                zipFile.Save(packageFile);
            }
        }

        protected static void AddProjectDirectory(ZipFile zipFile, string projectRootFolder, string directory, string customArchiveDirectory = null)
        {
            string directoryPath = Path.Combine(projectRootFolder, directory);

            if (Directory.Exists(directoryPath))
            {
                zipFile.AddDirectory(directoryPath, customArchiveDirectory ?? directory);
            }
        }

        protected static void AddProjectFile(ZipFile zipFile, string projectRootFolder, string file)
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
