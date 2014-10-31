using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlRepository
    {
        LoadSourcesResult LoadSources(SourceControl sourceControl, string version, string path);
    }
}