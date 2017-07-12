using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Dapper;

namespace AspNetDeploy.Dapper
{
    public class ProjectRepository
    {
        private readonly DapperDataContext dataContext;

        public ProjectRepository(IDataContext dataContext)
        {
            this.dataContext = (DapperDataContext)dataContext;
        }

        public IList<ProjectVersion> List()
        {
            Dictionary<int, ProjectVersion> projectVersionLookup = new Dictionary<int, ProjectVersion>();

            this.dataContext.Connection.Query<ProjectVersion, ProjectVersionProperty, Project, ProjectVersion>(@"
                    SELECT pv.Id Id, pv.Name Name, pv.ProjectId ProjectId, pv.SourceControlVersionId SourceControlVersionId, pv.ProjectTypeId ProjectType, pv.SolutionFile SolutionFile, pv.ProjectFile ProjectFile, pv.IsDeleted IsDeleted,
                    pvp.Id Id, pvp.ProjectVersionId ProjectVersionId, pvp.[Key] [Key], pvp.[Value] [Value],
                    p.*
                    FROM ProjectVersion pv
                    INNER JOIN ProjectVersionProperty pvp ON pv.Id = pvp.ProjectVersionId
                    INNER JOIN SourceControlVersion scv ON pv.SourceControlVersionId = scv.Id
                    INNER JOIN Project p ON pv.ProjectId = p.Id
                    WHERE scv.IsArchivedId != 2
                ", (pv, pvp, p) =>
                {
                    ProjectVersion projectVersion;

                    if (!projectVersionLookup.TryGetValue(pv.Id, out projectVersion))
                    {
                        projectVersionLookup.Add(pv.Id, projectVersion = pv);
                    }

                    if (pv.Properties == null)
                    {
                        projectVersion.Properties = new List<ProjectVersionProperty>();
                    }

                    pv.Project = p;

                    projectVersion.Properties.Add(pvp);

                    return pv;
                }).AsQueryable();

            return projectVersionLookup.Values.ToList();
        }
    }
}
