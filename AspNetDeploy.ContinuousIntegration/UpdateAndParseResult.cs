using System.Collections.Generic;

namespace AspNetDeploy.ContinuousIntegration
{
    public class UpdateAndParseResult
    {
        public bool HasChanges { get; set; }
        public IList<int> Projects { get; set; }

        public UpdateAndParseResult()
        {
            this.Projects = new List<int>();
        }
    }
}