namespace AspNetDeploy.Model
{
    public enum UserRoleAction
    {
        Undefined = 0,

        ReleaseApprove,
        ReleasePublishTest,
        ReleasePublishLive,

        VersionCreate,

        DeploymentChangeSteps,

        EnvironmentCreate,
        EnvironmentChangeVariables,

        ManageUsers
    }
}