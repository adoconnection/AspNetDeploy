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

        public void Start()
        {
            SourceControlManager sourceControlManager = Factory.GetInstance<SourceControlManager>();
            try
            {
                UpdateAndParseResult updateAndParseResult = sourceControlManager.UpdateAndParse(this.sourceControlVersionId);
                TaskRunnerContext.SetSourceControlVersionState(this.sourceControlVersionId, SourceControlState.Idle);
            }
            catch (Exception e)
            {
                TaskRunnerContext.SetSourceControlVersionState(this.sourceControlVersionId, SourceControlState.Error);
            }
        }

    }
}