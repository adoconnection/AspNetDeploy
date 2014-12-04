using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class DacpacDeploymentStepModel : ProjectRelatedDeploymentStepModel
    {
        public string StepTitle { get; set; }
        public string ConnectionString { get; set; }
        public string TargetDatabase { get; set; }
        public string CustomConfiguration { get; set; }
    }
}