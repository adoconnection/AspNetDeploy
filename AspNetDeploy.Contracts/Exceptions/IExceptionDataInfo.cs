namespace AspNetDeploy.Contracts.Exceptions
{
    public interface IExceptionDataInfo
    {
        bool IsProperty { get; }

        string Name { get; }

        string Value { get; }
    }
}