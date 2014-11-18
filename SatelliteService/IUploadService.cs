using System.IO;
using System.ServiceModel;

namespace SatelliteService
{
    [ServiceContract]
    public interface IUploadService
    {
        [OperationContract]
        bool UploadPackage(Stream stream);
    }
}