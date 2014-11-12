using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class WebSiteDeploymentStepModel : DeploymentStepModel
    {
        public BundleVersion ProjectVersion { get; set; }
        public string Destination { get; set; }

        [UIHint("BindingEditor")]
        public IEnumerable<Binding> Bindings { get; set; }
    }
}