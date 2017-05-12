namespace AspNetDeploy.Contracts
{
    public interface INpmPackageManager
    {
        void RestorePackages(string directory);
    }
}
