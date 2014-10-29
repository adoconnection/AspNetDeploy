using System.Collections;
using System.Collections.Generic;
using VisualStudioSolutionInfo;

namespace WebUI.Models
{
    public class SourceControlInfo
    {
        public SourceControl SourceControl { get; set; }
        public IList<VisualStudioSolution> Solutions { get; set; }

        public SourceControlInfo()
        {
            this.Solutions = new List<VisualStudioSolution>();
        }
    }
}
