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

        EnvironmentsManage,
        EnvironmentChangeVariables,

        ManageUsers,
        ViewLogs,

        SourceVersionsManage
    }
}