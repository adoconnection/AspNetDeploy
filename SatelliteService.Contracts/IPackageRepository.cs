namespace SatelliteService.Contracts
{
    public interface IPackageRepository
    {
        void ExtractProject(int projectId, string destination);
        void ExtractFiles(int archiveId, string destination);
    }
}