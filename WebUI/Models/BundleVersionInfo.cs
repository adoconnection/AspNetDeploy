using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class BundleVersionInfo
    {
        public BundleVersion BundleVersion { get; set; }
        public BundleState State { get; set; }
        public IList<ProjectVersionInfo> ProjectsVersionsInfo { get; set; }
    }
}