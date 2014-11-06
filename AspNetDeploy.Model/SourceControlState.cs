namespace AspNetDeploy.Model
{
    public enum SourceControlState
    {
        Undefined = 0,
        Idle = 10,
        PreparingToLoad = 20,
        Loading = 21,
        Stopping = 30,
        Error = 40
    }
}