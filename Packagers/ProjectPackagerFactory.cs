using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.Packagers.Zip;
using Packagers.VisualStudioProject;

namespace Packagers
{
    public class ProjectPackagerFactory : IProjectPackagerFactory
    {
        public IProjectPackager Create(ProjectType projectType)
        {
            if (projectType.HasFlag(ProjectType.Web))
            {
                return new WebProjectPackager();
            }

            if (projectType.HasFlag(ProjectType.Database))
            {
                return new DatabaseProjectPackager();
            }

            if (projectType.HasFlag(ProjectType.Console))
            {
                return new WebProjectPackager();
            }

            if (projectType.HasFlag(ProjectType.ZipArchive))
            {
                return new ZipProjectPackager();
            }

            throw new AspNetDeployException("Project type is not supported: " + projectType);
        }
    }
}
