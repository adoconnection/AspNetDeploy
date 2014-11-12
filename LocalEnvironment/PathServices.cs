using AspNetDeploy.Contracts;

namespace LocalEnvironment
{
    public class PathServices : IPathServices
    {
        public string GetSourceControlVersionPath(int sourceControlId, int sourceControlVersionId)
        {
            return string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\{1}", sourceControlId, sourceControlVersionId);
        }

        public string GetBundlePackagePath(int bundleId, int packageId)
        {
            return string.Format(@"H:\AspNetDeployWorkingFolder\Packages\package-{0}-{1}.zip", bundleId, packageId);
        }

        public string GetProjectPackagePath(int projectId, string revisionId)
        {
            return string.Format(@"H:\AspNetDeployWorkingFolder\Packages\project-{0}-{1}.zip", projectId, revisionId);
        }
    }
}
