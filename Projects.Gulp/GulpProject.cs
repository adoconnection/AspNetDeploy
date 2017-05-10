using System;

namespace Projects.Gulp
{
    public struct GulpProject
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public Guid Guid { get; set; }
    }
}