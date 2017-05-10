using System.IO;
using System.Text;
using AspNetDeploy.Contracts;
using Ionic.Zip;
using Ionic.Zlib;

namespace Packagers.Gulp
{
    public class GulpProjectPackager : IProjectPackager
    {
        public void Package(string projectPath, string packageFile)
        {
            string projectRootFolder = Path.GetDirectoryName(projectPath);
            string projectName = Path.GetFileNameWithoutExtension(projectPath).Replace(".gulpfile", "");

            using (ZipFile zipFile = new ZipFile(Encoding.UTF8))
            {
                zipFile.AlternateEncoding = Encoding.UTF8;
                zipFile.AlternateEncodingUsage = ZipOption.Always;

                zipFile.CompressionLevel = CompressionLevel.BestCompression;

                this.AddProjectDirectory(zipFile, projectRootFolder, projectName, "\\");

                zipFile.Save(packageFile);
            }
        }

        protected void AddProjectDirectory(ZipFile zipFile, string projectRootFolder, string directory, string customArchiveDirectory = null)
        {
            string directoryPath = Path.Combine(projectRootFolder, directory);

            if (Directory.Exists(directoryPath))
            {
                zipFile.AddDirectory(directoryPath, customArchiveDirectory ?? directory);
            }
        }
    }
}
