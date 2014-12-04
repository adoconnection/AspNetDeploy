using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class WebSiteDeploymentStepModel : ProjectRelatedDeploymentStepModel
    {
        public string SiteName { get; set; }
        public string Destination { get; set; }
        public string BindingsJson { get; set; }
    }
}