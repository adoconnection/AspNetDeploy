using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlProviderFactory
    {
        ISourceControlProvider Create(SourceControlType type);
    }
}