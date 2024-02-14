using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetDeploy.WebUI.Models
{
    public class MachineInstanceModel
    {
        public int Id { get; set; }
        public string PackagesPath { get; set; }
        public string BackupsPath { get; set; }
        public string Uri { get; set; }
        public bool IsAuthorizationEnabled { get; set; }
    }
}