using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using Ionic.Zip;

namespace AspNetDeploy.ContinuousIntegration
{
    public class PackageManager
    {
        private readonly IPathServices pathServices;
        private readonly IProjectPackagerFactory projectPackagerFactory;

        public PackageManager(IPathServices pathServices, IProjectPackagerFactory projectPackagerFactory)
        {
            this.pathServices = pathServices;
            this.projectPackagerFactory = projectPackagerFactory;
        }

        public void PackageBundle(int bundleVersionId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            BundleVersion bundleVersion = entities.BundleVersion
                .Include("Bundle")
                .Include("Packages")
                .Include("ProjectVersions.Project")
                .Include("ProjectVersions.SourceControlVersion.SourceControl")
                .First(bv => bv.Id == bundleVersionId);

            Package package = new Package
            {
                BundleVersion = bundleVersion,
                CreatedDate = DateTime.UtcNow
            };

            entities.Package.Add(package);

            using (ZipFile zipFile = new ZipFile(Encoding.UTF8))
            {
                zipFile.AlternateEncoding = Encoding.UTF8;
                zipFile.AlternateEncodingUsage = ZipOption.Always;

                foreach (ProjectVersion projectVersion in bundleVersion.ProjectVersions)
                {
                    string sourcesFolder = this.pathServices.GetSourceControlVersionPath(projectVersion.SourceControlVersion.SourceControl.Id, projectVersion.SourceControlVersion.Id);
                    string projectPackagePath = this.pathServices.GetProjectPackagePath(projectVersion.Id, projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                    string projectPath = Path.Combine(sourcesFolder, projectVersion.ProjectFile);
                   
                    IProjectPackager projectPackager = projectPackagerFactory.Create(projectVersion.ProjectType);

                    if (!File.Exists(projectPackagePath))
                    {
                        DateTime packageStartDate = DateTime.UtcNow;
                        projectPackager.Package(projectPath, projectPackagePath);
                        projectVersion.SetStringProperty("LastPackageDuration", (DateTime.UtcNow - packageStartDate).TotalSeconds.ToString(CultureInfo.InvariantCulture));
                        entities.SaveChanges();
                    }
                    
                    zipFile.AddFile(projectPackagePath, "/");

                    PackageEntry packageEntry = new PackageEntry
                    {
                        Package = package,
                        ProjectVersion = projectVersion,
                        Revision = projectVersion.SourceControlVersion.GetStringProperty("Revision")
                    };

                    entities.PackageEntry.Add(packageEntry);
                }

                zipFile.Save(this.pathServices.GetBundlePackagePath(bundleVersionId, package.Id));
            }

            package.PackageDate = DateTime.UtcNow;
            entities.SaveChanges();

            foreach (ProjectVersion projectVersion in bundleVersion.ProjectVersions)
            {
                string projectPackagePath = this.pathServices.GetProjectPackagePath(projectVersion.Id, projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                File.Delete(projectPackagePath);
            }
        }
    }
}