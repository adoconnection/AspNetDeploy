using System.IO;
using System.Linq;
using Ionic.Zip;
using SatelliteService.Contracts;

namespace SatelliteService.Packages
{
    public class FilePackageRepository : IPackageRepository
    {
        private readonly string packagePath;

        public FilePackageRepository(string packagePath)
        {
            this.packagePath = packagePath;
        }

        public void ExtractProject(int projectId, string destination)
        {
            using (ZipFile packageZipFile = ZipFile.Read(this.packagePath))
            {
                ZipEntry projectZipEntry = packageZipFile.Entries.First(e => e.FileName.StartsWith("project-" + projectId + "-"));

                using (MemoryStream extractedEntryStream = new MemoryStream())
                {
                    projectZipEntry.Extract(extractedEntryStream);
                    extractedEntryStream.Position = 0;

                    using (ZipFile projectZipFile = ZipFile.Read(extractedEntryStream))
                    {
                        projectZipFile.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
        }

        public void ExtractFiles(int archiveId, string destination)
        {
            using (ZipFile packageZipFile = ZipFile.Read(this.packagePath))
            {
                ZipEntry archiveZipEntry = packageZipFile.Entries.First(e => e.FileName.StartsWith("archive-" + archiveId + "-"));

                using (MemoryStream extractedEntryStream = new MemoryStream())
                {
                    archiveZipEntry.Extract(extractedEntryStream);
                    extractedEntryStream.Position = 0;

                    using (ZipFile projectZipFile = ZipFile.Read(extractedEntryStream))
                    {
                        projectZipFile.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
        }
    }
}