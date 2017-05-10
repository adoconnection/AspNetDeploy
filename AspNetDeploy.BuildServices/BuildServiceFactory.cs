using AspNetDeploy.BuildServices.MSBuild;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using BuildServices.Gulp;

namespace AspNetDeploy.BuildServices
{
    public class BuildServiceFactory : IBuildServiceFactory
    {
        private readonly INugetPackageManager nugetPackageManager;

        public BuildServiceFactory(INugetPackageManager nugetPackageManager)
        {
            this.nugetPackageManager = nugetPackageManager;
        }

        public IBuildService Create(ProjectType projectType)
        {
            if (projectType == ProjectType.GulpFile)
            {
                return new GulpBuildService();
            }

            return new MSBuildBuildService(this.nugetPackageManager);
        }
    }
}
