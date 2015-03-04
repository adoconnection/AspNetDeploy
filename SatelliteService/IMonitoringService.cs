using System.ServiceModel;

namespace SatelliteService
{
    [ServiceContract]
    public interface IMonitoringService
    {
        [OperationContract]
        ServerSummary GetServerSummary();
    }
}