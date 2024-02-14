namespace AspNetDeploy.Contracts
{
    public interface IVariableProcessorFactory
    {
        IVariableProcessor Create(int packageId, int machineId);
    }
}