using System;

namespace AspNetDeploy.Projects.Zip
{
    internal struct ZipProject
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Guid Guid { get; set; }
    }
}