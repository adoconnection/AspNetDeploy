using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Models.Setup;

namespace AspNetDeploy.WebUI.Controllers
{
    public class SetupController : GenericController
    {
        public SetupController(ILoggingService loggingService) : base(loggingService)
        {
        }

        public ActionResult ConnectionString()
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            ConnectionStringStepModel model = new ConnectionStringStepModel()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["AspNetDeployEntitiesClean"].ConnectionString
            };

            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            configuration.AppSettings.Settings["Settings.SetupState"].Value = "ConnectionString";
            configuration.Save();

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConnectionString(ConnectionStringStepModel model)
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            string connectionString = model.ConnectionString.Trim().TrimEnd(';') + "; Connection Timeout = 2";

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError("ConnectionString", e.Message);
                return this.View(model);
            }

            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            ConnectionStringsSection section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
            section.ConnectionStrings["AspNetDeployEntities"].ConnectionString = ConfigurationManager.ConnectionStrings["AspNetDeployEntitiesTemplate"].ConnectionString.Replace("{connectionString}", model.ConnectionString.Trim().TrimEnd(';'));
            section.ConnectionStrings["AspNetDeployEntitiesClean"].ConnectionString = model.ConnectionString.Trim();
            configuration.Save();

            return this.RedirectToAction("DatabaseStructure");
        }

        public ActionResult DatabaseStructure()
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            try
            {
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AspNetDeployEntitiesClean"].ConnectionString);
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError("ConnectionString", e.Message);
                return this.View(new DatabaseStructureStepModel()
                {
                    IsReady = false
                });
            }

            if (!this.Entities.Database.Exists())
            {
                return this.RedirectToAction("ConnectionString");
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatabaseStructure(DatabaseStructureStepModel model)
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AspNetDeployEntitiesClean"].ConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(SqlResource.InitializationScript.Replace("GO", ""), connection);
            command.ExecuteNonQuery();

            connection.Close();

            return this.RedirectToAction("WorkingFolder");
        }


        public ActionResult WorkingFolder()
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            WorkingFolderStepModel model = new WorkingFolderStepModel
            {
                Path = ConfigurationManager.AppSettings["Settings.WorkingFolder"]
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkingFolder(WorkingFolderStepModel model)
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            model.Path = model.Path.Trim();

            try
            {
                if (!Directory.Exists(model.Path))
                {
                    Directory.CreateDirectory(model.Path);
                }

                string testPath = Path.Combine(model.Path, "Test");
                string testFile = Path.Combine(testPath, "text.txt");

                Directory.CreateDirectory(testPath);
                System.IO.File.WriteAllText(testFile, "Test text");
                System.IO.File.Delete(testFile);
                Directory.Delete(testPath);

            }
            catch (Exception e)
            {
                this.ModelState.AddModelError("Path", e.Message);
                return this.View(model);
            }


            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            configuration.AppSettings.Settings["Settings.WorkingFolder"].Value = model.Path;
            configuration.Save();

            return this.RedirectToAction("Users");
        }

        public ActionResult Users()
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Users(UsersStepModel model)
        {
            if (!this.IsSetupState())
            {
                return this.Redirect("/");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            User user = this.Entities.User.FirstOrDefault( u => u.Role == UserRole.Admin);

            if (user == null)
            {
                user = new User();
                this.Entities.User.Add(user);
                user.Guid = Guid.NewGuid();
            }

            user.Name = "Administrator";
            user.Email = model.Email;
            user.IsDisabled = false;
            user.Password = model.Password;
            user.Role = UserRole.Admin;
            
            this.Entities.SaveChanges();

            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            configuration.AppSettings.Settings["Settings.SetupState"].Value = "";
            configuration.AppSettings.Settings["Settings.EnableTaskRunner"].Value = "true";
            configuration.Save();

            FormsAuthentication.SetAuthCookie(user.Guid.ToString(), true);

            return this.Redirect("/");
        }

    }
}