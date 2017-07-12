using System;
using System.Collections.Generic;
using System.Data;
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
            IDictionary<int, Bundle> bundles = new Dictionary<int, Bundle>();
            IDictionary<int, BundleVersion> bundleVersions = new Dictionary<int, BundleVersion>();

            this.dataContext.Connection.Query<Bundle, BundleVersion, BundleVersionProperty, Bundle>(@"
SELECT
	b.*,
	bv.*,
    bvp.*
FROM [Bundle] b
LEFT JOIN [BundleVersion] bv ON bv.BundleId = b.Id
LEFT JOIN [BundleVersionProperty] bvp ON bvp.BundleVersionId = bv.Id
WHERE bv.Id IN 
(
	SELECT TOP 2 bv2.Id
	FROM BundleVersion bv2
	WHERE bv2.BundleId = bv.BundleId
	ORDER BY LEN(bv2.Name) DESC, bv2.Name
)
ORDER by b.OrderIndex

                ", (b, bv, bvp) =>
            {
                Bundle bundle;

                if (!bundles.TryGetValue(b.Id, out bundle))
                {
                    bundles.Add(b.Id, b);
                    bundle = b;
                }

                if (bundle.BundleVersions == null)
                {
                    bundle.BundleVersions = new List<BundleVersion>();
                }

                if (bundle.BundleVersions == null)
                {
                    bundle.BundleVersions = new List<BundleVersion>();
                }

                BundleVersion bundleVersion;

                if (!bundleVersions.TryGetValue(bv.Id, out bundleVersion))
                {
                    bundleVersions.Add(bv.Id, bv);
                    bundleVersion = bv;
                }

                if (bundleVersion.Properties == null)
                {
                    bundleVersion.Properties = new List<BundleVersionProperty>();
                }

                bundleVersion.Properties.Add(bvp);

                return b;
            }).AsQueryable();

            return bundles.Values.ToList();
        }
    }
}