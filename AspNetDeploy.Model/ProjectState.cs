namespace AspNetDeploy.Model
{
    public enum ProjectState
    {
        Undefined = 0,
        Idle = 10,
        QueuedToBuild = 20,
        Building = 21,
        Stopping = 30,
        Error = 40
    }
}