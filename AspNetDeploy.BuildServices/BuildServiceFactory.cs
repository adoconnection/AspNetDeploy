using AspNetDeploy.BuildServices.MSBuild;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using BuildServices.Gulp;

namespace AspNetDeploy.BuildServices
{
    public class BuildServiceFactory : IBuildServiceFactory
    {
        private readonly IPathServices pathServices;

        public BuildServiceFactory(IPathServices pathServices)
        {
            this.pathServices = pathServices;
        }

        public IBuildService Create(ProjectType projectType)
        {
            if (projectType == ProjectType.GulpFile)
            {
                return new GulpBuildService(this.pathServices);
            }

            return new MSBuildBuildService(this.pathServices);
        }
    }
}
