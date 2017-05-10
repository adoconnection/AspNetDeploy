using AspNetDeploy.SolutionParsers.VisualStudio;

namespace AspNetDeploy.Projects.VisualStudio2013
{
    internal struct VsProject
    {
        public string SolutionFile { get; set; }
        public VisualStudioSolutionProject Project { get; set; }
    }
}