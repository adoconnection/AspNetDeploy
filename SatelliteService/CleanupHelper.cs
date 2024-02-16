using System;
using System.IO;

namespace SatelliteService
{
    public class CleanupHelper
    {
        public static void CleanupFolder(string daysConfigKey, string path)
        {
            int days = 10;

            if (!string.IsNullOrWhiteSpace(daysConfigKey))
            {
                int.TryParse(daysConfigKey, out days);
            }

            DateTime dateTime = DateTime.Now.AddDays(-days);

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(file);

                if (fi.CreationTime < dateTime)
                {
                    fi.Delete();
                }
            }
        }
    }
}