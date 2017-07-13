using System.Collections.Generic;

namespace AspNetDeploy.Model
{
    public class BundleVersionComparer : IEqualityComparer<BundleVersion>
    {
        public bool Equals(BundleVersion x, BundleVersion y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(BundleVersion obj)
        {
            return obj.Id;
        }
    }
}