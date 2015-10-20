using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public void ExtractProject(int projectId, string destination, Action<string, bool> beforeExtracting = null)
        {
            ReadOptions options = new ReadOptions();
            options.Encoding = Encoding.UTF8;

            using (ZipFile packageZipFile = ZipFile.Read(this.packagePath, options))
            {
                ZipEntry projectZipEntry = packageZipFile.Entries.First(e => e.FileName.StartsWith("project-" + projectId + "-"));

                using (MemoryStream extractedEntryStream = new MemoryStream())
                {
                    projectZipEntry.Extract(extractedEntryStream);
                    extractedEntryStream.Position = 0;

                    using (ZipFile projectZipFile = ZipFile.Read(extractedEntryStream, options))
                    {
                        projectZipFile.ExtractProgress += (sender, args) =>
                        {
                            if (args.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
                            {
                                if (beforeExtracting != null)
                                {
                                    beforeExtracting(args.ExtractLocation, args.CurrentEntry.IsDirectory);
                                }
                            }
                        };

                        projectZipFile.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently | ExtractExistingFileAction.InvokeExtractProgressEvent);
                    }
                }
            }
        }

        public IList<string> ListFiles(int projectId)
        {
            ReadOptions options = new ReadOptions();
            options.Encoding = Encoding.UTF8;

            IList<string> result = new List<string>();

            using (ZipFile packageZipFile = ZipFile.Read(this.packagePath, options))
            {
                ZipEntry projectZipEntry = packageZipFile.Entries.First(e => e.FileName.StartsWith("project-" + projectId + "-"));

                using (MemoryStream extractedEntryStream = new MemoryStream())
                {
                    projectZipEntry.Extract(extractedEntryStream);
                    extractedEntryStream.Position = 0;

                    using (ZipFile projectZipFile = ZipFile.Read(extractedEntryStream, options))
                    {
                        this.ListEntriesRecursive(result, "/", projectZipFile.Entries);
                    }
                }
            }

            return result;
        }

        public Stream LoadProjectFile(int projectId, string file)
        {
            using (ZipFile packageZipFile = new ZipFile(this.packagePath))
            {
                packageZipFile.AlternateEncoding = Encoding.UTF8;
                packageZipFile.AlternateEncodingUsage = ZipOption.Always;

                ZipEntry projectZipEntry = packageZipFile.Entries.First(e => e.FileName.StartsWith("project-" + projectId + "-"));

                using (MemoryStream extractedEntryStream = new MemoryStream())
                {
                    projectZipEntry.Extract(extractedEntryStream);
                    extractedEntryStream.Position = 0;

                    ReadOptions options = new ReadOptions();
                    options.Encoding = Encoding.UTF8;

                    using (ZipFile projectZipFile = ZipFile.Read(extractedEntryStream, options))
                    {
                        projectZipFile.AlternateEncoding = Encoding.UTF8;
                        projectZipFile.AlternateEncodingUsage = ZipOption.Always;

                        ZipEntry projectFileEntry = projectZipFile.First( e => e.FileName.ToLower() == file.ToLower());

                        MemoryStream projectfileStream = new MemoryStream();
                        projectFileEntry.Extract(projectfileStream);
                        projectfileStream.Position = 0;

                        return projectfileStream;
                    }
                }
            }
        }

        private void ListEntriesRecursive(IList<string> result, string parentPath, IEnumerable<ZipEntry> entries)
        {
            foreach (ZipEntry entry in entries)
            {
                result.Add(entry.FileName);

            }
        }


    }
}