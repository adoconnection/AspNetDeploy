namespace AspNetDeploy.Contracts.Entities
{
    public interface ISourceControl : IPropertyContainer
    {
        int Id { get; }
        string DisplayName { get; }
        string TypeId { get; }
        bool IsDeleted { get; }
        int OrderIndex { get; }
    }
}