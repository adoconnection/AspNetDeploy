using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Enums;
using Ionic.Zip;
using Ionic.Zlib;
using Newtonsoft.Json;
using SatelliteService.Contracts;

namespace SatelliteService.LocalBackups
{
    public class LocalBackupRepository : IBackupRepository
    {
        public Guid StoreObject(object obj)
        {
            Guid guid = Guid.NewGuid();

            /*File.WriteAllText(this.GetBackupFileContentPath(guid), JsonConvert.SerializeObject(obj));*/

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(this.GetBackupFileContentPath(guid), FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, obj);
            }

            return guid;
        }

        public Guid StoreFile(string file)
        {
            Guid guid = Guid.NewGuid();

            File.WriteAllText(this.GetBackupFileInfoPath(guid), file);
            File.Copy(file, this.GetBackupFileContentPath(guid), true);

            return guid;
        }

        public Guid StoreDirectory(string path)
        {
            Guid guid = Guid.NewGuid();

            using (ZipFile zipFile = new ZipFile())
            {
                CompressionLevel compressionLevel;

                zipFile.CompressionLevel = Enum.TryParse(ConfigurationManager.AppSettings["LocalBackups.CompressionLevel"], out compressionLevel) ? compressionLevel : CompressionLevel.BestCompression;
                zipFile.AddDirectory(path, "/");
                zipFile.Save(this.GetBackupFileContentPath(guid));
            }

            File.WriteAllText(this.GetBackupFileInfoPath(guid), path);

            return guid;
        }

        public void RestoreFile(Guid guid, string file = null)
        {
            string originalLocation = File.ReadAllText(this.GetBackupFileInfoPath(guid));
            File.Copy(this.GetBackupFileContentPath(guid), file ?? originalLocation, true);
        }

        public void RestoreDirectory(Guid guid, string path = null)
        {
            string originalLocation = File.ReadAllText(this.GetBackupFileInfoPath(guid));

            this.DeleteContents(path ?? originalLocation);

            using (ZipFile zipFile = ZipFile.Read(this.GetBackupFileContentPath(guid)))
            {
                zipFile.ExtractAll(path ?? originalLocation, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public T RestoreObject<T>(Guid guid)
        {
            /*return JsonConvert.DeserializeObject<T>(File.ReadAllText(this.GetBackupFileContentPath(guid)));*/

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(this.GetBackupFileContentPath(guid), FileMode.Open, FileAccess.Read))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        private string GetBackupFileContentPath(Guid guid)
        {
            return Path.Combine(ConfigurationManager.AppSettings["BackupsPath"], guid + ".dat");
        }
        private string GetBackupFileInfoPath(Guid guid)
        {
            return Path.Combine(ConfigurationManager.AppSettings["BackupsPath"], guid + ".txt");
        }

        private void DeleteContents(string path)
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