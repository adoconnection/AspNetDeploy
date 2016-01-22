namespace AspNetDeploy.Contracts
{
    public interface INugetPackageManager
    {
        void RestoreSolutionPackages(string solutionFile);
        void RestoreProjectPackages(string projectDirectory, string solutionDirectory);
    }
}