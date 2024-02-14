using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineServices
{
    public class MachineConfigModel
    {
        public string PackagesPath { get; set; }
        public string BackupsPath { get; set; }
        public string Uri { get; set; }
        public bool IsAuthorizationEnabled { get; set; }
    }
}
