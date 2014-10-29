using System.Collections.Generic;

namespace VisualStudioSolutionInfo
{
    public class VisualStudioSolution
    {
        public IList<VisualStudioProject> Projects { get; set; }

        public VisualStudioSolution()
        {
            this.Projects = new List<VisualStudioProject>();
        }
    }
}