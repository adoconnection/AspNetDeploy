using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace SourceControls.FileSystem
{
    public class FileSystemSourceControlRepository : ISourceControlRepository
    {
        public LoadSourcesResult LoadSources(SourceControlVersion sourceControlVersion, string path)
        {
            return new LoadSourcesResult
            {
                RevisionId = "2014-01-01"
            };
        }
    }
}
