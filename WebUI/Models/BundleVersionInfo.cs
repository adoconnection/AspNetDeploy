using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class BundleVersionInfo
    {
        public IList<Environment> Environments { get; set; }
        public BundleVersion BundleVersion { get; set; }
        public BundleState State { get; set; }
        public IList<ProjectVersionInfo> ProjectsVersionsInfo { get; set; }
        public IDictionary<int, List<Publication>> Publications { get; set; } = new Dictionary<int, List<Publication>>();
    }
}