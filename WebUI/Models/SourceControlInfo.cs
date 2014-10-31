using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class SourceControlInfo
    {
        public SourceControl SourceControl { get; set; }
        public SourceControlState State { get; set; }
    }
}