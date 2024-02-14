using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Dapper;

namespace AspNetDeploy.Dapper
{
    public class SourceRepository
    {
        private readonly DapperDataContext dataContext;

        public SourceRepository(IDataContext dataContext)
        {
            this.dataContext = (DapperDataContext)dataContext;
        }

        public IList<SourceControl> List(bool excludeArchived = true)
        {
            Dictionary<int, SourceControl> sourceControlLookup = new Dictionary<int, SourceControl>();

            this.dataContext.Connection.Query<SourceControl, SourceControlVersion, SourceControl>(@"
                    SELECT 
                        sc.Id, 
                        sc.Name, 
                        sc.Type AS Type, 
                        sc.IsDeleted, 
                        sc.OrderIndex, 
                        scv.Id, 
                        scv.SourceControlId, 
                        scv.ParentVersionId, 
                        scv.Name, 
                        scv.OrderIndex, 
                        scv.IsHead, 
                        scv.IsArchivedId AS ArchiveState
                    FROM SourceControl sc
                    LEFT JOIN SourceControlVersion scv ON sc.Id = scv.SourceControlId
                    WHERE 
                        (@excludeArchived = 0 OR (scv.IsArchivedId <> 2 AND @excludeArchived = 1))
                ", (sc, scv) =>
                {
                    SourceControl sourceControl;

                    if (!sourceControlLookup.TryGetValue(sc.Id, out sourceControl))
                    {
                        sourceControlLookup.Add(sc.Id, sourceControl = sc);
                    }

                    if (sc.SourceControlVersions == null)
                    {
                        sourceControl.SourceControlVersions = new List<SourceControlVersion>();
                    }

                    sourceControl.SourceControlVersions.Add(scv);

                    return sc;
                },
                new
                {
                    excludeArchived
                }).AsQueryable();

            return sourceControlLookup.Values.ToList();
        }
    }
}
