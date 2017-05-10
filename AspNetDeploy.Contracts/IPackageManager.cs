using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetDeploy.Contracts
{
    public interface IPackageManager
    {
        void RestorePackages(string directory);
    }
}
