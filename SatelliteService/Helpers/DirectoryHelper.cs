using System.IO;

namespace SatelliteService.Helpers
{
    public static class DirectoryHelper
    {
        public static void DeleteContents(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

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