using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models;

namespace AspNetDeploy.WebUI.Controllers
{
    public class AccountController : GenericController
    {
        public AccountController(ILoggingService loggingService) : base(loggingService)
        {
        }

        [HttpGet]
        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            User user = this.Entities.User.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || user.Password != model.Password || user.IsDisabled)
            {
                this.ModelState.AddModelError("Password", "Неправильное имя или пароль");
                return this.View(model);
            }

            FormsAuthentication.SetAuthCookie(user.Guid.ToString(), true);
            return this.Redirect("/");
        }

        [Authorize]
        public ActionResult LogOut()
        {
            //this.ActiveUser.SecurityKey = Guid.NewGuid().ToString();
            this.Entities.SaveChanges();

            FormsAuthentication.SignOut();
            return this.Redirect("/");
        }


        [Authorize]
        [OutputCache(Duration = 100, VaryByParam = "id")]
        public ActionResult Avatar(int id)
        {
            User user = this.Entities.User.FirstOrDefault(u => u.Id == id);

            string hash = "00000000000000000000000000000000";

            if (user != null)
            {
                hash = GetMD5(user.Email);
            }

            return this.Redirect("https://www.gravatar.com/avatar/" + hash + ".jpg?d=mm&s=30");
        }

        private string GetMD5(string inputString)
        {
            byte[] hashBytes = (MD5.Create()).ComputeHash(Encoding.Default.GetBytes(inputString));
            StringBuilder builder = new StringBuilder();

            foreach (byte hashByte in hashBytes)
            {
                builder.Append(hashByte.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}