namespace AspNetDeploy.WebUI.Models
{
    public class ConfigDeploymentStepModel : DeploymentStepModel
    {
        public string ConfigJson { get; set; }
        public string StepTitle { get; set; }
        public string File { get; set; }
    }
}