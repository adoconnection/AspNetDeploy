using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class ZipArchiveDeploymentStepModel : DeploymentStepModel
    {
        public string StepTitle { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public string Destination { get; set; }
        public string CustomConfigurationJson { get; set; }
    }
}