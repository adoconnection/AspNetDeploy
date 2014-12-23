namespace AspNetDeploy.Model
{
    public enum UserRoleAction
    {
        Undefined = 0,

        ReleaseApprove,
        ReleasePublishTest,
        ReleasePublishLive,
        ReleaseCancel,

        VersionCreate,

        DeploymentChangeSteps,

        EnvironmentCreate,
        EnvironmentChangeVariables,

        ManageUsers,
        ViewLogs
    }
}