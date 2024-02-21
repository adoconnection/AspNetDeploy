using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class UsersController : AuthorizedAccessController
    {
        public UsersController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult List()
        {
            this.CheckPermission(UserRoleAction.ManageUsers);

            this.ViewBag.Users = this.Entities.User.ToList();

            return this.View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            this.CheckPermission(UserRoleAction.ManageUsers);

            User user = this.Entities.User.First(u => u.Id == id);

            UserEditModel model = new UserEditModel()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsDisabled = user.IsDisabled,
                Role = user.Role,
                ThemeId = user.ThemeId ?? ConfigurationManager.AppSettings["DefaultTheme"]
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditModel model)
        {
            this.CheckPermission(UserRoleAction.ManageUsers);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }


            if (this.Entities.User.Any(u => u.Email == model.Email.Trim() && u.Id != model.Id))
            {
                this.ModelState.AddModelError("Email", "Этот адрес уже занят");
                return this.View(model);
            }

            User user = this.Entities.User.First(u => u.Id == model.Id);

            user.Name = model.Name;
            user.Email = model.Email;
            user.ThemeId = model.ThemeId;

            user.Role = model.Role;

            if (model.Id != this.ActiveUser.Id)
            {
                user.Role = model.Role;
                user.IsDisabled = model.IsDisabled;
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = model.Password;
            }

            this.Entities.SaveChanges();

            return this.RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Add()
        {
            this.CheckPermission(UserRoleAction.ManageUsers);
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserAddModel model)
        {
            this.CheckPermission(UserRoleAction.ManageUsers);

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (this.Entities.User.Any(u => u.Email == model.Email.Trim()))
            {
                this.ModelState.AddModelError("Email", "Этот адрес уже занят");
                return this.View(model);
            }

            User user = new User();

            user.Name = model.Name;
            user.Email = model.Email;
            user.IsDisabled = model.IsDisabled;
            user.Password = model.Password;
            user.ThemeId = model.ThemeId;

            user.Guid = Guid.NewGuid();

            user.Role = model.Role;

            this.Entities.User.Add(user);
            this.Entities.SaveChanges();

            return this.RedirectToAction("List");
        }
    }
}