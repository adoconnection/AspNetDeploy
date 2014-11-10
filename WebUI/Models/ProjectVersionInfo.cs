using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class ProjectVersionInfo
    {
        public ProjectVersion ProjectVersion { get; set; }
        public ProjectState ProjectState { get; set; }
    }
}