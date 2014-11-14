using System;
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

            Package package = new Package();
            package.BundleVersion = bundleVersion;
            package.CreatedDate = DateTime.UtcNow;

            entities.Package.Add(package);
            entities.SaveChanges();

            using (ZipFile zipFile = new ZipFile(Encoding.UTF8))
            {
                foreach (ProjectVersion projectVersion in bundleVersion.ProjectVersions)
                {
                    string sourcesFolder = this.pathServices.GetSourceControlVersionPath(projectVersion.SourceControlVersion.SourceControl.Id, projectVersion.SourceControlVersion.Id);
                    string projectPackagePath = this.pathServices.GetProjectPackagePath(projectVersion.Id, projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                    string projectPath = Path.Combine(sourcesFolder, projectVersion.ProjectFile);
                   
                    IProjectPackager projectPackager = projectPackagerFactory.Create(projectVersion.ProjectType);

                    if (!File.Exists(projectPackagePath))
                    {
                        projectPackager.Package(projectPath, projectPackagePath); 
                    }
                    
                    zipFile.AddFile(projectPackagePath, "/");
                }

                zipFile.Save(this.pathServices.GetBundlePackagePath(bundleVersionId, package.Id));
            }

            foreach (ProjectVersion projectVersion in bundleVersion.ProjectVersions)
            {
                string projectPackagePath = this.pathServices.GetProjectPackagePath(projectVersion.Id, projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                File.Delete(projectPackagePath);
            }
        }
    }
}