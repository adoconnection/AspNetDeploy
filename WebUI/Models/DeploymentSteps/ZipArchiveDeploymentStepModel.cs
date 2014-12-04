using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class ZipArchiveDeploymentStepModel : ProjectRelatedDeploymentStepModel
    {
        public string StepTitle { get; set; }
        public string Destination { get; set; }
        public string CustomConfigurationJson { get; set; }
    }
}