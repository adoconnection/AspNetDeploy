namespace AspNetDeploy.Contracts
{
    public interface IPathServices
    {
        string GetSourceControlVersionPath(int sourceControlId, int sourceControlVersionId);
        string GetBundlePackagePath(int bundleId, int packageId);
        string GetProjectPackagePath(int projectId, string revisionId);
        string GetNugetPath();
        string GetNpmPath();
    }
}