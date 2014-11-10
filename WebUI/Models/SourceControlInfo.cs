using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class SourceControlInfo
    {
        public SourceControl SourceControl { get; set; }
        public IList<SourceControlVersionInfo> SourceControlVersionsInfo { get; set; }
    }
}