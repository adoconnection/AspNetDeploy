using System.Linq;
using AspNetDeploy.Model;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Serializers
{
    public class SourceControlSerializer
    {
        public object SerializeDetails(SourceControl sourceControl)
        {
            return new
            {
                id = sourceControl.Id,
                name = sourceControl.Name,
                type = sourceControl.Type,
                properties = sourceControl.Properties.Select(scp => new { scp.Key, scp.Value }).ToList()
            };
        }

        public object SerializeDeleted(SourceControl sourceControl)
        {
            return new
            {
                id = sourceControl.Id
            };
        }
    }
}