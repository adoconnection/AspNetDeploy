using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class BundleInfo
    {
        public Bundle Bundle { get; set; }
        public BundleState State { get; set; }
        public IList<ProjectInfo> ProjectsInfo { get; set; }
    }
}