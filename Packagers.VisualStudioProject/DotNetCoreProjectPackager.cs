using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ionic.Zip;

namespace Packagers.VisualStudioProject
{
    public class DotNetCoreProjectPackager : VisualStudioProjectPackager
    {
        protected override void PackageProjectContents(ZipFile zipFile, XDocument xDocument, XNamespace vsNamespace, string projectRootFolder)
        {
            XElement targetFramework = xDocument.Descendants("TargetFramework").FirstOrDefault();

            if (targetFramework == null)
            {
                throw new VisualStudioPackagerException("targetFramework not set");
            }

            if (!this.IsFrameworkSupported(targetFramework.Value))
            {
                throw new VisualStudioPackagerException("targetFramework not supported: " + targetFramework.Value);
            }

            AddProjectDirectory(
                zipFile,
                projectRootFolder,
                Path.Combine("bin", "Release", targetFramework.Value, "publish"),
                "\\");
        }

        private bool IsFrameworkSupported(string targetFramework)
        {
            if (targetFramework.Equals("netcoreapp2.0", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (targetFramework.Equals("netcoreapp2.1", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (targetFramework.Equals("netcoreapp2.2", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (targetFramework.Equals("netcoreapp2.3", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}