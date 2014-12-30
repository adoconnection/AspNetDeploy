using System;
using System.Globalization;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner.Jobs
{
    public class PackageJob
    {
        public void Start(int bundleId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            BundleVersion bundleVersion = entities.BundleVersion
                .Include("ProjectVersions.Properties")
                .Include("ProjectVersions.SourceControlVersion.Properties")
                .Include("Properties")
                .First(bv => bv.Id == bundleId);

            DateTime packageStart = DateTime.UtcNow;

            PackageManager packageManager = Factory.GetInstance<PackageManager>();
            packageManager.PackageBundle(bundleId);

            foreach (ProjectVersion projectVersion in bundleVersion.ProjectVersions)
            {
                projectVersion.SetStringProperty("LastPackageRevision", projectVersion.SourceControlVersion.GetStringProperty("Revision"));
                projectVersion.SetStringProperty("LastPackageDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }

            bundleVersion.SetStringProperty("LastPackageDuration", (DateTime.UtcNow - packageStart).TotalSeconds.ToString(CultureInfo.InvariantCulture));

            entities.SaveChanges();
        }
    }
}