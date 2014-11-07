using System.Collections;
using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class SourceControlInfo
    {
        public SourceControl SourceControl { get; set; }
        public IList<ProjectInfo> ProjectsInfo { get; set; }
        public SourceControlState State { get; set; }
    }
}