using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.SourceControls.Git
{
    public class GitSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            throw new System.NotImplementedException();
        }

        public void LoadSources(string version, string path)
        {
            throw new System.NotImplementedException();
        }
    }
}