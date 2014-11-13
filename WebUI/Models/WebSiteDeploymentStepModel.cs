using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class WebSiteDeploymentStepModel : DeploymentStepModel
    {
        public string SiteName { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public string Destination { get; set; }
        public string BindingsJson { get; set; }
    }
}