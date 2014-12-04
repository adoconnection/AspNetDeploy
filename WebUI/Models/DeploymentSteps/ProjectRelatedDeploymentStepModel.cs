using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class ProjectRelatedDeploymentStepModel : DeploymentStepModel
    {
        [Required]
        public int ProjectId { get; set; }
    }
}