using AspNetDeploy.BuildServices.MSBuild;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.BuildServices
{
    public class BuildServiceFactory : IBuildServiceFactory
    {
        public IBuildService Create(SolutionType project)
        {
            return new MSBuildBuildService();
        }
    }
}
