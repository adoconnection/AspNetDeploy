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

        public IList<ProjectVersion> ListForSources()
        {
            Dictionary<int, ProjectVersion> projectVersionLookup = new Dictionary<int, ProjectVersion>();

            this.dataContext.Connection.Query<ProjectVersion, ProjectVersionProperty, Project, ProjectVersion>(@"
SELECT 
    pv.Id, 
    pv.Name, 
    pv.ProjectId, 
    pv.SourceControlVersionId, 
    pv.ProjectTypeId ProjectType, 
    pv.SolutionFile, 
    pv.ProjectFile, 
    pv.IsDeleted,
    pvp.Id, 
    pvp.ProjectVersionId, 
    pvp.[Key] [Key], 
    pvp.[Value] [Value],
    p.*
FROM ProjectVersion pv
JOIN Project p ON pv.ProjectId = p.Id
LEFT JOIN ProjectVersionProperty pvp ON pv.Id = pvp.ProjectVersionId
LEFT JOIN SourceControlVersion scv ON pv.SourceControlVersionId = scv.Id
WHERE scv.IsArchivedId != 2 OR scv.IsArchivedId IS NULL
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

                    if (pvp != null && pvp.Key != null)
                    {
                        projectVersion.Properties.Add(pvp);
                    }

                    return pv;
                }).ToList();

            return projectVersionLookup.Values.ToList();
        }

        public IDictionary<int, List<int>> ListBundleVersionProjects(params int[] bundleVersionIds)
        {
            return this.dataContext.Connection.Query(@"
SELECT 
    m.ProjectVersionId,
    m.BundleVersionId
FROM ProjectVersionToBundleVersion m
WHERE
    m.BundleVersionId IN @bundleVersionIds
",
                new
                {
                    bundleVersionIds
                }).GroupBy(k => k.BundleVersionId).ToDictionary(k => (int) k.Key, v => v.Select(e => (int) e.ProjectVersionId).ToList());
        }

        public IList<ProjectVersion> ListForBundles(params int[] bundleVersionIds)
        {
            Dictionary<int, ProjectVersion> projectVersionLookup = new Dictionary<int, ProjectVersion>();

            this.dataContext.Connection.Query<ProjectVersion, ProjectVersionProperty, Project, ProjectVersion>(@"
SELECT 
    bv.Id BundleVersionId,
    pv.Id, 
    pv.Name, 
    pv.ProjectId, 
    pv.SourceControlVersionId,
    pv.ProjectTypeId ProjectType, 
    pv.SolutionFile, 
    pv.ProjectFile, 
    pv.IsDeleted,
    pvp.*,
    p.*
FROM ProjectVersion pv
LEFT JOIN ProjectVersionProperty pvp ON pv.Id = pvp.ProjectVersionId
LEFT JOIN ProjectVersionToBundleVersion bv2pv ON bv2pv.ProjectVersionId = pv.Id
LEFT JOIN BundleVersion bv ON bv.Id = bv2pv.BundleVersionId
LEFT JOIN Project p ON pv.ProjectId = p.Id
WHERE 
    bv.IsDeleted = 0 AND
    bv.Id IN @bundleVersionIds
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
            }, new
            {
                bundleVersionIds
            }).AsQueryable();

            return projectVersionLookup.Values.ToList();
        }
    }
}
