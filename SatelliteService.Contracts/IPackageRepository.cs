using System;
using System.Collections.Generic;
using System.IO;

namespace SatelliteService.Contracts
{
    public interface IPackageRepository
    {
        void ExtractProject(int projectId, string destination, Action<string, bool> beforeExtracting = null);
        IList<string> ListFiles(int projectId);
        Stream LoadProjectFile(int projectId, string file);
    }
}