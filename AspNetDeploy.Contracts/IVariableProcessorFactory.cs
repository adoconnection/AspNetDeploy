namespace AspNetDeploy.Contracts
{
    public interface IVariableProcessorFactory
    {
        IVariableProcessor Create(int bundleVersionId, int machineId);
    }
}