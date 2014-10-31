using System.ComponentModel;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public class SourceControlJob
    {
        private readonly ISourceControlRepositoryFactory repositoryFactory;

        public SourceControlState Status { get; set; }

        public BackgroundWorker Worker { get; set; }

        public SourceControlJob(ISourceControlRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
            this.Status = SourceControlState.Idle;
            this.Worker = new BackgroundWorker();
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
        }

        public void Start(SourceControl sourceControl)
        {
            this.Status = SourceControlState.Loading;

            this.Worker.RunWorkerAsync(sourceControl);
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Status = SourceControlState.Error;
            }
            else
            {
                this.Status = SourceControlState.Idle;    
            }
            
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SourceControl sourceControl = (SourceControl)e.Argument;

            ISourceControlRepository repository = repositoryFactory.Create(sourceControl.Type);
            e.Result = repository.LoadSources(sourceControl, "trunk", string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\trunk", sourceControl.Id));
        }
    }
}