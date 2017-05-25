using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Model;
using AspNetDeploy.SourceControls;

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
            SourceControlModelFactory sourceControlModelFactory = new SourceControlModelFactory();

            List<SourceControl> sourceControls = this.Entities.SourceControl
                    .Include("Properties")
                    .Where(sc => !sc.IsDeleted)
                    .ToList();

            this.TransmitConnection(
                "App/SourceControls/List",
                sourceControls
                    .Select(sc => sourceControlModelFactory.Create(sc.Type).ListSerializer(sc))
                    .ToList());
        }
    }
}