using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts.MachineSummary;

namespace AspNetDeploy.DeploymentServices.WCFSatellite.MonitoringServiceReference
{
    public partial class ServerSummary : IServerSummary
    {
        IList<IServerUpdateInfo> IServerSummary.Updates
        {
            get
            {
                return this.Updates.Cast<IServerUpdateInfo>().ToList();
            }
        }

        IList<IDriveInfo> IServerSummary.Drives
        {
            get
            {
                return this.Drives.Cast<IDriveInfo>().ToList();
            }
        }
    }
}