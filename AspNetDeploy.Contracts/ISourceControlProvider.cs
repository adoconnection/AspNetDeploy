using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISourceControlProvider
    {
        TestSourceResult TestConnection(SourceControlVersion sourceControlVersion);
        LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path);
        void Archive(SourceControlVersion sourceControlVersion, string path);
    }
}