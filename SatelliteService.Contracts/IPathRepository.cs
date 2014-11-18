namespace SatelliteService.Contracts
{
    public interface IPathRepository
    {
        string GetPackagePath(int publicationId);
    }
}