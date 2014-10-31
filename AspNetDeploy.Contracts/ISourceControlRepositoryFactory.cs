using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlRepositoryFactory
    {
        ISourceControlRepository Create(SourceControlType type);
    }
}