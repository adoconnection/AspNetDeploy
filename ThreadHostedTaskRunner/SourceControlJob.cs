using System;
using System.ComponentModel;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner
{
    public class SourceControlJob
    {
        private int sourceControlVersionId;

        public BackgroundWorker Worker { get; set; }

        public SourceControlJob()
        {
            this.Worker = new BackgroundWorker();
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.Worker.ProgressChanged += this.Worker_ProgressChanged;
        }

        public void Start(int sourceControlId)
        {
            this.sourceControlVersionId = sourceControlId;
            this.Worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TaskRunnerContext.SetSourceControlVersionState(
                this.sourceControlVersionId, 
                e.Error == null 
                    ? SourceControlState.Idle 
                    : SourceControlState.Error);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SourceControlManager sourceControlManager = Factory.GetInstance<SourceControlManager>();
            UpdateAndParseResult updateAndParseResult = sourceControlManager.UpdateAndParse(this.sourceControlVersionId);
/*
            if (updateAndParseResult.HasChanges)
            {
                foreach (int projectId in updateAndParseResult.Projects)
                {
                    TaskRunnerContext.SetNeedToBuildProject(projectId, true);
                }
            }*/
        }
    }
}