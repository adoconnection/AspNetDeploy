using System.ServiceModel;

namespace SatelliteService
{
    [ServiceContract]
    public interface IInformationService
    {
        [OperationContract]
        int GetVersion();
    }
}