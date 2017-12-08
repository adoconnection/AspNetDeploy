namespace AspNetDeploy.WebUI.Models.DeploymentSteps
{
    public class RunVsTestStepModel : ProjectRelatedDeploymentStepModel
    {
        public string StepTitle { get; set; }
        public bool StopOnFailure { get; set; }
        public string FiltersJson { get; set; }
    }
}