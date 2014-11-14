namespace AspNetDeploy.Model
{
    public enum BundleState
    {
        Undefined = 0,
        Idle = 10,
        LoadingQueued = 20,
        Loading = 21,
        BuildQueued = 30,
        Building = 31,
        TestingQueued = 40,
        Testing = 41,
        PackagingQueued = 50,
        Packaging = 51,
        DeployingQueued = 60,
        Deploying = 61
    }
}