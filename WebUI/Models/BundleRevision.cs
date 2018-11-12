using System;

namespace AspNetDeploy.WebUI.Models
{
    public class BundleRevision
    {
        public string SourceName { get; set; }
        public string Author { get; set; }
        public string Commit { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}