
namespace AspNetDeploy.WebUI.Models
{
    public class DeploymentStepModel
    {
        public int OrderIndex { get; set; }
        public int BundleVersionId { get; set; }
        public int DeploymentStepId { get; set; }
        public string Roles { get; set; }
    }
}