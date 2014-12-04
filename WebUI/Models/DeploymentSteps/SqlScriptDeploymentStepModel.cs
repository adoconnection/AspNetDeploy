using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class SqlScriptDeploymentStepModel : DeploymentStepModel
    {
        public string StepTitle { get; set; }
        [Required]
        public string Command { get; set; }
        public string Variables { get; set; }
        [Required]
        public string ConnectionString { get; set; }
    }
}