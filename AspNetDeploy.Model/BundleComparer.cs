using System.Collections.Generic;

namespace AspNetDeploy.Model
{
    public class BundleComparer : IEqualityComparer<Bundle>
    {
        public bool Equals(Bundle x, Bundle y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Bundle obj)
        {
            return obj.Id;
        }
    }
}