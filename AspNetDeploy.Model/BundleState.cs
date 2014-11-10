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
        DeployingQueued = 50,
        Deploying = 51
    }
}