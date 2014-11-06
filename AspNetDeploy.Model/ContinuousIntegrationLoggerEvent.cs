namespace AspNetDeploy.Model
{
    public enum ContinuousIntegrationLoggerEvent
    {
        Undefined = 0,
        SolutionBuildStarted,
        ProjectBuildStarted,
        ProjectBuildComplete,
        ProjectBuildFailed,
        SolutionBuildComplete,
        SolutionBuildFailed
    }
}