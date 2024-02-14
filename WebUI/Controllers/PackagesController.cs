using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.WebUI.Controllers
{
    public class PackagesController : AuthorizedAccessController
    {
        public PackagesController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult Details(int id)
        {
            Package package = this.Entities.Package
                .Include("Publications.Environment")
                .Include("BundleVersion.Bundle")
                .Include("BundleVersion.Packages")
                .Include("PackageEntry.ProjectVersion.SourceControlVersion.Revisions.RevisionInfos")
                .Include("ApprovedOnEnvironments")
                .First(p => p.Id == id);

            IList<Environment> environments = this.Entities.Environment
                .Include("NextEnvironment")
                .Include("Publications.Package")
                .ToList();

            int homeEnvironment = package.BundleVersion.GetIntProperty("HomeEnvironment");

            if (homeEnvironment > 0)
            {
                environments = environments.First(e => e.Id == homeEnvironment).GetNextEnvironments();
            }
            else
            {
                environments = new List<Environment>(); 
            }

            this.ViewBag.Package = package;
            this.ViewBag.Environments = environments;

            IList<BundleRevision> revisions = new List<BundleRevision>();

            foreach (IGrouping<SourceControlVersion, PackageEntry> group in package.PackageEntry.GroupBy(e => e.ProjectVersion.SourceControlVersion))
            {
                // TODO: create releation to Revision from PackageEntry
                Revision revision = group.Key.Revisions.FirstOrDefault(r => r.Name == group.First().Revision);

                if (revision == null)
                {
                    continue;
                }

                foreach (RevisionInfo revisionInfo in revision.RevisionInfos.OrderByDescending(i => i.CreatedDate))
                {
                    revisions.Add(new BundleRevision
                    {
                        CreatedDate = revisionInfo.CreatedDate,
                        Author = revisionInfo.Author,
                        Commit = revisionInfo.Message,
                        SourceName = group.Key.SourceControl.Name
                    });
                }
            }

            this.ViewBag.Revisions = revisions.OrderByDescending(r => r.CreatedDate).ToList();

            return this.View();
        }

        public ActionResult Approve(int id, int environmentid)
        {
            Package package = this.Entities.Package
                .Include("ApprovedOnEnvironments")
                .First(p => p.Id == id);

            Environment environment = this.Entities.Environment.First( e => e.Id == environmentid);

            if (!environment.GetBoolProperty("AllowTestDeployment", false))
            {
                this.CheckPermission(UserRoleAction.ReleaseApprove);
            }

            if (package.ApprovedOnEnvironments.Any(a => a.Environment == environment))
            {
                return this.RedirectToAction("Details", new {id});
            }

            PackageApprovedOnEnvironment onEnvironment = new PackageApprovedOnEnvironment();

            onEnvironment.Package = package;
            onEnvironment.Environment = environment;
            onEnvironment.ApprovedDate = DateTime.UtcNow;
            onEnvironment.ApprovedByUserId = this.ActiveUser.Id;

            this.Entities.PackageApprovedOnEnvironment.Add(onEnvironment);
            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new { id });
        }

        public ActionResult Reject(int id, int environmentid)
        {
            Package package = this.Entities.Package
                .Include("ApprovedOnEnvironments.ApprovedBy")
                .First(p => p.Id == id);

            Environment environment = this.Entities.Environment.First(e => e.Id == environmentid);

            if (!environment.GetBoolProperty("AllowTestDeployment", false))
            {
                this.CheckPermission(UserRoleAction.ReleaseApprove);
            }

            PackageApprovedOnEnvironment packageApprovedOnEnvironment = package.ApprovedOnEnvironments.FirstOrDefault(a => a.Environment == environment);

            if (packageApprovedOnEnvironment == null)
            {
                return this.RedirectToAction("Details", new { id });
            }

            this.Entities.PackageApprovedOnEnvironment.Remove(packageApprovedOnEnvironment);
            this.Entities.SaveChanges();

            return this.RedirectToAction("Details", new { id });
        }

        public ActionResult Schedule(int id, int environmentid)
        {
            Environment environment = this.Entities.Environment
                .Include("Properties")
                .First( e => e.Id == environmentid);

            if (environment.GetBoolProperty("AllowTestDeployment"))
            {
                this.CheckPermission(UserRoleAction.ReleasePublishTest);
            }
            else
            {
                this.CheckPermission(UserRoleAction.ReleasePublishLive);
            }

            Package package = this.Entities.Package.First( p => p.Id == id);

            Publication publication = new Publication();
            publication.CreatedDate = DateTime.UtcNow;
            publication.EnvironmentId = environmentid;
            publication.PackageId = id;
            publication.State = PublicationState.Queued;
            this.Entities.Publication.Add(publication);
            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionPackages", "Bundles", new { id = package.BundleVersionId });
        }

        public ActionResult Cancel(int id, int environmentid)
        {
            this.CheckPermission(UserRoleAction.ReleaseCancel);

            Publication publication = this.Entities.Publication
                .Include("Package")
                .Where( p => p.PackageId == id && p.EnvironmentId == environmentid).OrderByDescending( p => p.CreatedDate).FirstOrDefault();

            if (publication == null || publication.State != PublicationState.Queued)
            {
                return this.RedirectToAction("VersionPackages", "Bundles", new { id, environmentid });
            }

            publication.State = PublicationState.Canceled;
            this.Entities.SaveChanges();

            return this.RedirectToAction("VersionPackages", "Bundles", new { id = publication.Package.BundleVersionId });
        }
    }
}