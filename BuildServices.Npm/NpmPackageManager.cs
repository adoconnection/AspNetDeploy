using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetDeploy.Contracts;

namespace BuildServices.Npm
{
    public class NpmPackageManager : IPackageManager
    {
        public void RestorePackages(string directory)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(directory);

            process.StartInfo.FileName = "npm.exe";
            process.StartInfo.Arguments = "install";

            process.Start();

            process.WaitForExit();
        }
    }
}
