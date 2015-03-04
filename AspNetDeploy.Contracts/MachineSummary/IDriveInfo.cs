namespace AspNetDeploy.Contracts.MachineSummary
{
    public interface IDriveInfo
    {
        string Name { get; }

        string Label { get; }

        double TotalSpaceMB { get; }

        double FreeSpaceMB { get; } 
    }
}