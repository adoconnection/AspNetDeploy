namespace AspNetDeploy.Model
{
    public enum MachineState
    {
        Undefined = 0,
        Idle = 10,
        DeployingQueued = 20,
        Deploing = 21,
        Uploading = 31,
        Installing = 32,
        Finalizing = 33,
        Error = 100
    }
}