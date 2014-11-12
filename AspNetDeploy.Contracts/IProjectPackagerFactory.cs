using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface IProjectPackagerFactory
    {
        IProjectPackager Create(ProjectType projectType);
    }
}