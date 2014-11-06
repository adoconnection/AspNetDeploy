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
        private int sourceControlId;

        public BackgroundWorker Worker { get; set; }

        public SourceControlJob()
        {
            this.Worker = new BackgroundWorker();
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
        }

        public void Start(int sourceControlId)
        {
            this.sourceControlId = sourceControlId;
            this.Worker.RunWorkerAsync();
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TaskRunnerContext.SetSourceControlState(
                this.sourceControlId, 
                e.Error == null 
                    ? SourceControlState.Idle 
                    : SourceControlState.Error);
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SourceControlManager sourceControlManager = Factory.GetInstance<SourceControlManager>();
            UpdateAndParseResult updateAndParseResult = sourceControlManager.UpdateAndParse(sourceControlId);

            if (updateAndParseResult.HasChanges)
            {
                foreach (int projectId in updateAndParseResult.Projects)
                {
                    TaskRunnerContext.NeedToBuildProject(projectId, true);
                }
            }
        }
    }
}