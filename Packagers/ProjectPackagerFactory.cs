using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using Packagers.VisualStudioProject;

namespace Packagers
{
    public class ProjectPackagerFactory : IProjectPackagerFactory
    {
        public IProjectPackager Create(ProjectType projectType)
        {
            if (projectType.HasFlag(ProjectType.Web))
            {
                return new VisualStudioProjectPackager();
            }

            throw new AspNetDeployException("Project type is not supported: " + projectType);
        }
    }
}
