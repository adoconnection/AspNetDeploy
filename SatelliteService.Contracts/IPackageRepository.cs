namespace SatelliteService.Contracts
{
    public interface IPackageRepository
    {
        void ExtractProject(int projectId, string destination);
    }
}