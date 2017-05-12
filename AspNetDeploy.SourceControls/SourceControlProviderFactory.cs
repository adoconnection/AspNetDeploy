using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls.Git;
using AspNetDeploy.SourceControls.SVN;
using SourceControls.FileSystem;

namespace AspNetDeploy.SourceControls
{
    public class SourceControlProviderFactory : ISourceControlProviderFactory
    {
        public ISourceControlProvider Create(SourceControlType type)
        {
            switch (type)
            {
                case SourceControlType.Svn:
                    return new SvnSourceControlRepository();

                case SourceControlType.Git:
                    return new GitSourceControlRepository();

                case SourceControlType.FileSystem:
                    return new FileSystemSourceControlRepository();

                default:
                    throw new AspNetDeployException("Unknown SourceControlType: " + type);
            }
        }
    }
}
