namespace SatelliteService.Contracts
{
    public interface IPackageRepositoryFactory
    {
        IPackageRepository Create(string path);
    }
}