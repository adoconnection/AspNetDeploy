using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;

namespace SourceControls.FileSystem
{
    public class FileSystemSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            string workingDirectory;

            if (sourceControlVersion.SourceControl.GetBoolProperty("IsRelativeMode"))
            {
                workingDirectory = Path.Combine(sourceControlVersion.SourceControl.GetStringProperty("Path"), sourceControlVersion.GetStringProperty("Path"));
            }
            else
            {
                workingDirectory = sourceControlVersion.SourceControl.GetStringProperty("Path");
            }

            List<string> filePaths = Directory.GetFiles(workingDirectory, "*.zip", SearchOption.TopDirectoryOnly).ToList();

            string revisionId = string.Join(
                ";", 
                filePaths
                    .Select( f => 
                        f.GetHashCode().ToString(CultureInfo.InvariantCulture) +
                        File.GetLastWriteTimeUtc(f).ToString(CultureInfo.InvariantCulture)))
                .GetHashCode()
                .ToString(CultureInfo.InvariantCulture);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (sourceControlVersion.GetStringProperty("Revision") != revisionId) // yeah, dirty
            {
                DeleteContents(path);
                foreach (string filePath in filePaths)
                {
                    File.Copy(filePath, Path.Combine(path, Path.GetFileName(filePath)));
                }
            }

            return new LoadSourcesResult
            {
                RevisionId = revisionId
            };
        }

        public void Archive(SourceControlVersion sourceControlVersion, string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static void DeleteContents(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}
;