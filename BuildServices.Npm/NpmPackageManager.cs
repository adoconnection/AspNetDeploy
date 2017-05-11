using System;
using System.Collections.Generic;
using System.Configuration;
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
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = directory;

            process.StartInfo.FileName = ConfigurationManager.AppSettings["Settings.NpmBinary"];
            process.StartInfo.Arguments = "install";

            process.Start();

            process.WaitForExit();
        }
    }
}
