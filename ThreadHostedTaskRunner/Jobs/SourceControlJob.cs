using System;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner.Jobs
{
    public class SourceControlJob
    {
        private readonly int sourceControlVersionId;

        public SourceControlJob(int sourceControlVersionId)
        {
            this.sourceControlVersionId = sourceControlVersionId;
        }

        public void UpdateAndParse()
        {
            SourceControlManager sourceControlManager = Factory.GetInstance<SourceControlManager>();
            UpdateAndParseResult updateAndParseResult = sourceControlManager.UpdateAndParse(this.sourceControlVersionId);
        }

        public void Archive()
        {
            SourceControlManager sourceControlManager = Factory.GetInstance<SourceControlManager>();
            ArhiveResult arhiveResult = sourceControlManager.Archive(this.sourceControlVersionId);
        }

    }
}