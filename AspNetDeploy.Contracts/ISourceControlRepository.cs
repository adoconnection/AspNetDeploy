using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlRepository
    {
        LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path);
        void Archive(SourceControlVersion sourceControlVersion, string path);
    }
}