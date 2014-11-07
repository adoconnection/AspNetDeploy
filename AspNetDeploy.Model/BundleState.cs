namespace AspNetDeploy.Model
{
    public enum BundleState
    {
        Undefined = 0,
        Idle = 10,
        Loading = 20,
        BuildQueued = 30,
        Building = 31,
        Testing = 40,
        Deploying = 50
    }
}