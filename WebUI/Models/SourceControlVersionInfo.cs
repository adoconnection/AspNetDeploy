using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class SourceControlVersionInfo
    {
        public SourceControlVersion SourceControlVersion { get; set; }
        public SourceControlState State { get; set; }
        public IList<ProjectVersionInfo> ProjectVersionsInfo { get; set; }
    }
}