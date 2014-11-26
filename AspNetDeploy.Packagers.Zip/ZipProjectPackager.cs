using System.IO;
using AspNetDeploy.Contracts;

namespace AspNetDeploy.Packagers.Zip
{
    public class ZipProjectPackager : IProjectPackager
    {
        public void Package(string projectPath, string packageFile)
        {
            File.Copy(projectPath, packageFile, true);
        }
    }
}
