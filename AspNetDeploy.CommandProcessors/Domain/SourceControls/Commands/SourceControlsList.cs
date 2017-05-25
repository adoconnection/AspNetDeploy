using System.Linq;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlsList : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/List";
            }
        }

        public override void Process()
        {
            this.TransmitConnection(
                "App/SourceControls/List",
                this.Entities.SourceControl
                    .Where(sc => !sc.IsDeleted)
                    .Select(Serializers.SourceControlSerializer.SerializeDetails)
                    .ToList());
        }
    }
}