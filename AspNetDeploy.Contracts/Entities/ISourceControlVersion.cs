namespace AspNetDeploy.Contracts.Entities
{
    public interface ISourceControlVersion : IPropertyContainer
    {
        int Id { get; }
        int SourceControlId { get; }
        int ParentVersionId { get; }
        string DisplayName { get; }
        bool IsDeleted { get; }
        int OrderIndex { get; }
        bool IsArchived { get; }
    }
}