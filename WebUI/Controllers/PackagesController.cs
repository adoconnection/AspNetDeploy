using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Model;
using Environment = AspNetDeploy.Model.Environment;

namespace AspNetDeploy.WebUI.Controllers
{
    public class PackagesController : AuthorizedAccessController
    {
        public ActionResult Details(int id)
        {
            Package package = this.Entities.Package
                .Include("Publications.Environment")
                .Include("BundleVersion.Bundle")
                .Include("BundleVersion.Packages")
                .Include("ApprovedOnEnvironments")
                .First(p => p.Id == id);

            List<Environment> environments = this.Entities.Environment.ToList();

            this.ViewBag.Package = package;
            this.ViewBag.Environments = environments;

            return this.View();
        }

        public ActionResult Approve(int id, int environmentid)
        {
            this.CheckPermission(UserRoleAction.ReleaseApprove);

            Package package = this.Entities.Package
                .Include("ApprovedOnEnvironments")
                .First(p => p.Id == id);

            Environment environment = this.Entities.Environment.First( e => e.Id == environmentid);

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
            this.CheckPermission(UserRoleAction.ReleaseApprove);

            Package package = this.Entities.Package
                .Include("ApprovedOnEnvironments.ApprovedBy")
                .First(p => p.Id == id);

            Environment environment = this.Entities.Environment.First(e => e.Id == environmentid);

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
    }
}