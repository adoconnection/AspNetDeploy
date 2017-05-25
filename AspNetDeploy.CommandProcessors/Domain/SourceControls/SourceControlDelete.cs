using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls
{
    public class SourceControlDelete : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Delete";
            }
        }

        public override void Process()
        {
            if (!this.HasPermission(UserRoleAction.SourceVersionsManage))
            {
                return;
            }

            int id = this.Data.id;
            string safeWord = this.Data.safeWord;

            if (safeWord != "DELETE")
            {
                this.TrnsmitUnableToExecute("SafeWordRequired", id);
                return;
            }

            SourceControl sourceControl = this.Entities.SourceControl.FirstOrDefault(sc => sc.Id == id && !sc.IsDeleted);

            if (sourceControl == null)
            {
                this.TrnsmitUnableToExecute("SourceControlNotFound", id);
                return;
            }

            sourceControl.IsDeleted = true;

            this.Entities.SaveChanges();

            this.TransmitAllUsers(
                "App/SourceControls/Delete",
                new
                {
                    id = sourceControl.Id
                });
        }
    }
}