using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class ProjectVersionInfo
    {
        public ProjectVersion ProjectVersion { get; set; }
        public SourceControlVersion SourceControlVersion { get; set; }
        public SourceControl SourceControl { get; set; }
    }
}