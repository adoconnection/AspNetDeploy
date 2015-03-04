using System;
using System.Linq;
using WUApiLib;

namespace SatelliteService
{
    public class MonitoringService : IMonitoringService
    {
        public ServerSummary GetServerSummary()
        {
            ServerSummary serverSummary = new ServerSummary();

            this.FillMemoryInfo(serverSummary);
            this.FillDrivesInfo(serverSummary);
            this.FillUpdatesInfo(serverSummary);

            return serverSummary;
        }

        private void FillUpdatesInfo(ServerSummary serverSummary)
        {
            UpdateSession updateSession = new UpdateSession();

            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            updateSearcher.Online = false;

            ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And IsHidden=0");

            serverSummary.Updates = searchResult.Updates.Cast<IUpdate>().Select(u => new ServerUpdateInfo
            {
                Title = u.Title,
                ReleaseDate = u.LastDeploymentChangeTime,
                IsDownloaded = u.IsDownloaded,
                IsMandatory = u.IsMandatory
            }).ToList();
        }

        private void FillDrivesInfo(ServerSummary serverSummary)
        {
            serverSummary.Drives = System.IO.DriveInfo.GetDrives()
                .Select(di => new DriveInfo()
                {
                    Name = di.Name,
                    Label = di.VolumeLabel,
                    FreeSpaceMB = Math.Round((double) di.AvailableFreeSpace/1024/1024),
                    TotalSpaceMB = Math.Round((double) di.TotalSize/1024/1024)
                })
                .ToList();
        }

        private void FillMemoryInfo(ServerSummary serverSummary)
        {
            MemoryHelper memoryHelper = new MemoryHelper();
            serverSummary.AvailableMemoryMB = memoryHelper.GetAvailableMemoryMB();
            serverSummary.TotalMemoryMB = memoryHelper.GetTotalRamMB();
        }
    }
}