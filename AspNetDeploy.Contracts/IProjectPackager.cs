namespace AspNetDeploy.Contracts
{
    public interface IProjectPackager
    {
        void Package(string projectPath, string packageFile);
    }
}