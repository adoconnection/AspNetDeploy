using System;

namespace VisualStudioSolutionInfo
{
    public class VisualStudioProject
    {
        public string Name { get; set; }
        public string ProjectFile { get; set; }
        public Guid Guid { get; set; }
        public Guid TypeGuid { get; set; }
        public VisualStudioProjectType Type { get; set; }
    }
}