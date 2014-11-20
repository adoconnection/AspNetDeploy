using SatelliteService.Contracts;

namespace SatelliteService.Packages
{
    public class PackageRepositoryFactory : IPackageRepositoryFactory
    {
        public IPackageRepository Create(string path)
        {
            return new FilePackageRepository(path);
        }
    }
}