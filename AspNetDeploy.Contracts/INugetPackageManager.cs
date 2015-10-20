namespace AspNetDeploy.Contracts
{
    public interface INugetPackageManager
    {
        void RestorePackages(string packagesConfigPath, string solutionDirectory);
        void RestorePackages(string solutionFile);
    }
}