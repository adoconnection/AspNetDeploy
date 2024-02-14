using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetDeploy.WebUI.Models
{
    public class MachineInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Environments { get; set; }

    }
}