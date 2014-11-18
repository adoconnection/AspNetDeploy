using System.Configuration;
using System.IO;
using SatelliteService.Contracts;

namespace SatelliteService.LocalEnvironment
{
    public class PathRepository : IPathRepository
    {
        public string GetPackagePath(int publicationId)
        {
            return Path.Combine(ConfigurationManager.AppSettings["PackagesPath"], "publication-" + publicationId + ".zip");
        }
    }
}