using System.Collections.Generic;

namespace AspNetDeploy.Contracts.MachineSummary
{
    public interface IServerSummary
    {
        IList<IDriveInfo> Drives { get; }

        IList<IServerUpdateInfo> Updates { get; }

        double AvailableMemoryMB { get; }
        double TotalMemoryMB { get; }
    }
}