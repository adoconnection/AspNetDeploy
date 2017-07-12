using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IList<SourceControl> List()
        {
            Dictionary<int, SourceControl> sourceControlLookup = new Dictionary<int, SourceControl>();
            this.dataContext.Connection.Query<SourceControl, SourceControlVersion, SourceControl>(@"
                    SELECT sc.Id Id, sc.Name Name, sc.TypeId Type, sc.IsDeleted IsDeleted, sc.OrderIndex OrderIndex, 
                    scv.Id Id, scv.SourceControlId SourceControlId, scv.ParentVersionId ParentVersionId, scv.Name Name, scv.OrderIndex OrderIndex, scv.IsHead IsHead, scv.IsArchivedId ArchiveState
                    FROM SourceControl sc
                    INNER JOIN SourceControlVersion scv ON sc.Id = scv.SourceControlId
                    WHERE scv.IsArchivedId != 2
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
            }).AsQueryable();

            return sourceControlLookup.Values.ToList();
        }
    }
}
