using System.ServiceModel;

namespace SatelliteService
{
    public interface IMonitoringService
    {
        
        ServerSummary GetServerSummary();
    }
}