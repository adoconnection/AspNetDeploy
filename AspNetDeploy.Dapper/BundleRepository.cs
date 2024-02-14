using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Dapper;

namespace AspNetDeploy.Dapper
{
    public class BundleRepository
    {
        private readonly DapperDataContext dataContext;

        public BundleRepository(IDataContext dataContext)
        {
            this.dataContext = (DapperDataContext) dataContext;
        }

        public IList<Bundle> List()
        {
            List<Tuple<Bundle, BundleVersion, BundleVersionProperty>> list = dataContext.Connection.Query<Bundle, BundleVersion, BundleVersionProperty, Tuple<Bundle, BundleVersion, BundleVersionProperty>>(@"
                    SELECT
	                    b.*,
	                    bv.*,
                        bvp.*
                    FROM [Bundle] b
                    JOIN [BundleVersion] bv ON bv.BundleId = b.Id
                    JOIN [BundleVersionProperty] bvp ON bvp.BundleVersionId = bv.Id
                    WHERE 
                        b.IsDeleted = 0 AND
                        bv.IsDeleted = 0 /* AND
                        bv.Id IN 
                        (
	                        SELECT TOP 2 bv2.Id
	                        FROM BundleVersion bv2
	                        WHERE bv2.BundleId = bv.BundleId
	                        ORDER BY cast('/' + bv2.Name + '/' as hierarchyid
                        ) */
                    ORDER by b.OrderIndex
                    "
                , (o, o1, arg3) => new Tuple<Bundle, BundleVersion, BundleVersionProperty>(o, o1, arg3)).ToList();

            List<Bundle> bundles = list.GroupBy(
                        k => k.Item1,
                        new BundleComparer())
                    .Select(b =>
                    {
                        b.Key.BundleVersions = b
                                .GroupBy(k => k.Item2, v => v.Item3, new BundleVersionComparer())
                                .Select(bv =>
                                {
                                    bv.Key.Properties = bv.ToList();
                                    return bv.Key;
                                }).ToList();
                        return b.Key;
                    })
                    .ToList();


            return bundles;
        }
    }
}