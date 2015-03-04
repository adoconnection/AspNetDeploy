using System;

namespace AspNetDeploy.Contracts.MachineSummary
{
    public interface IServerUpdateInfo
    {
        string Title { get; }
        string Description { get; }

        DateTime ReleaseDate { get; }

        bool IsMandatory { get; }

        bool IsDownloaded { get; }
    }
}