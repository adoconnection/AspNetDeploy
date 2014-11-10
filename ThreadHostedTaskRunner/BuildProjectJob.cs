using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner
{
    public class BuildProjectJob
    {
        private int sourceControlId;
        private string solutionFileName;

        public BackgroundWorker Worker { get; set; }

        public BuildProjectJob()
        {
            this.Worker = new BackgroundWorker();
            this.Worker.WorkerSupportsCancellation = true;
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
        }
        public void Start(int sourceControlId, string solutionFileName)
        {
            this.sourceControlId = sourceControlId;
            this.solutionFileName = solutionFileName;
            this.Worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControl sourceControl = entities.SourceControl
                .Include("Properties")
                .First(sc => sc.Id == sourceControlId);

            List<Project> projects = entities.Project
                .Include("Bundles")
                .ToList();

            BuildManager buildManager = Factory.GetInstance<BuildManager>();
            buildManager.Build(
                this.sourceControlId, 
                solutionFileName,
                projectId => TaskRunnerContext.SetProjectVersionState(projectId, ProjectState.Building),
                (projectId, isSuccess) =>
                {
                    TaskRunnerContext.SetProjectVersionState(projectId, isSuccess ? ProjectState.Idle : ProjectState.Error);
                    TaskRunnerContext.SetNeedToBuildProject(projectId, false);

                    foreach (Bundle bundle in projects.First(p => p.Id == projectId).Bundles)
                    {
                        if (TaskRunnerContext.GetBundleVersionState(bundle.Id) == BundleState.Building && bundle.Projects.All(p => TaskRunnerContext.GetProjectVersionState(p.Id) == ProjectState.Idle))
                        {
                            TaskRunnerContext.SetBundleVersionState(bundle.Id, BundleState.Idle);
                        }

                    }

                });


            sourceControl.SetStringProperty("LastBuildRevision", sourceControl.GetStringProperty("Revision"));
            sourceControl.SetStringProperty("LastBuildDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            entities.SaveChanges();
        }
    }
}