using System.IO;

namespace SatelliteService.Contracts
{
    public interface IPackageRepository
    {
        void ExtractProject(int projectId, string destination);
        Stream LoadProjectFile(int projectId, string file);
    }
}