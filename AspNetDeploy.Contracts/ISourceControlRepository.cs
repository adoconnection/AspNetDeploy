using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlRepository
    {
        TestSourceResult TestConnection(SourceControlVersion sourceControlVersion);
        LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path);
        LoadSourcesInfoResult LoadSourcesInfo(SourceControlVersion sourceControlVersion, string path);
        void Archive(SourceControlVersion sourceControlVersion, string path);
    }
}