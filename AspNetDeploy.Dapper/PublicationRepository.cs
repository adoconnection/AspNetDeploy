using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Dapper;

namespace AspNetDeploy.Dapper
{
    public class PublicationRepository
    {
        private readonly DapperDataContext dataContext;

        public PublicationRepository(IDataContext dataContext)
        {
            this.dataContext = (DapperDataContext)dataContext;
        }

        public IList<Publication> ListForBundles(params int[] bundleVersionIds)
        {
            ConcurrentDictionary<int, Environment> environments = new ConcurrentDictionary<int, Environment>();
            ConcurrentDictionary<int, Package> packages = new ConcurrentDictionary<int, Package>();

            return this.dataContext.Connection
                .Query<Publication, Environment, Package, Publication>(@"
                    SELECT 
                        pub.*,
                        pub.ResultId State,
	                    e.*,
	                    pack.*
                    FROM Publication pub
                    JOIN Environment e ON e.Id = pub.EnvironmentId
                    JOIN Package pack ON pack.Id = pub.PackageId
                    WHERE 
                        pack.BundleVersionId IN @bundleVersionIds
                ", (pub, e, pack) =>
                {

                    pub.Package = packages.GetOrAdd(pack.Id, pack);
                    pub.Environment = environments.GetOrAdd(e.Id, e);

                    return pub;

                }, new
                {
                    bundleVersionIds
                })
            .ToList();

        }
    }
}