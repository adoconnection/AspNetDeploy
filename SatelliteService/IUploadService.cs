using System.IO;
using System.ServiceModel;

namespace SatelliteService
{
    public interface IUploadService
    {
        
        bool UploadPackage(Stream stream);
    }
}