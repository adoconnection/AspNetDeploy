using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls.Git;
using AspNetDeploy.SourceControls.SVN;

namespace AspNetDeploy.SourceControls
{
    public class SourceControlRepositoryFactory : ISourceControlRepositoryFactory
    {
        public ISourceControlRepository Create(SourceControlType type)
        {
            switch (type)
            {
                case SourceControlType.Svn:
                    return new SvnSourceControlRepository();

                case SourceControlType.Git:
                    return new GitSourceControlRepository();

                default:
                    throw new AspNetDeployException("Unknown SourceControlType: " + type);
            }
        }
    }
}
