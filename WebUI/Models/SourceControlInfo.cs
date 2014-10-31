using System.Collections.Generic;
using AspNetDeploy.Model;
using VisualStudioSolutionInfo;

namespace AspNetDeploy.WebUI.Models
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
