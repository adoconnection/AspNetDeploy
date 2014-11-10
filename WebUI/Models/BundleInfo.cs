using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class BundleInfo
    {
        public Bundle Bundle { get; set; }
        public IList<BundleVersionInfo> BundleVersionsInfo { get; set; }
    }
}