namespace AspNetDeploy.Model
{
    public enum SourceControlState
    {
        Undefined = 0,
        Idle = 10,
        LoadingQueued = 20,
        Loading = 21,
        Stopping = 30,
        Error = 40,
        LockedByBundle = 100,

        ScheduledForArchive = 200,
        Archiving = 201
    }
}