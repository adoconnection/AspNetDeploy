namespace AspNetDeploy.Contracts
{
    public interface ISourceControlRepository
    {
        void LoadSources(string version, string path);
    }
}