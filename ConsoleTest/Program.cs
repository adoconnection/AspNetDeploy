using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Bootstrapper;
using ObjectFactory;
using ThreadHostedTaskRunner;

namespace ConsoleTest
{
    class Program
    {
        private static bool WorkerComplete = false;

        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            //PackageManager packageManager = Factory.GetInstance<PackageManager>();
            //packageManager.PackageBundle(4);

            RunScheduler();

            Console.WriteLine("Complete");
            Console.ReadKey();
        }

        private static void RunScheduler()
        {
            

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, eventArgs) => ThreadTaskRunner.ProcessTasks();
            worker.RunWorkerCompleted += (sender, eventArgs) =>
            {
                Console.WriteLine("Worker Complete");
                WorkerComplete = true;
            };
            worker.RunWorkerAsync();

            AspNetDeployEntities entities = new AspNetDeployEntities();

            while (true)
            {
                Console.WriteLine(DateTime.Now);
                List<SourceControlVersion> sourceControls = entities.SourceControlVersion.Include("SourceControl").ToList();
                List<BundleVersion> bundleVersions = entities.BundleVersion.Include("Bundle").ToList();
                List<ProjectVersion> projectVersions = entities.ProjectVersion.Include("Project").ToList();

                foreach (SourceControlVersion sourceControlVersion in sourceControls)
                {
                    Console.WriteLine(sourceControlVersion.Id + " - " + sourceControlVersion.SourceControl.Name + " / " + sourceControlVersion.Name + " - " + TaskRunnerContext.GetSourceControlVersionState(sourceControlVersion.Id));
                }

                Console.WriteLine("");

                foreach (BundleVersion bundleVersion in bundleVersions)
                {
                    Console.WriteLine(bundleVersion.Id + " - " + bundleVersion.Bundle.Name + " / " + bundleVersion.Name + " - " + TaskRunnerContext.GetBundleVersionState(bundleVersion.Id));
                }

                Console.WriteLine("");

                foreach (ProjectVersion projectVersion in projectVersions.Where(p => TaskRunnerContext.GetProjectVersionState(p.Id) != ProjectState.Idle))
                {
                    Console.WriteLine(
                        projectVersion.SourceControlVersion.SourceControl.Name + "/ " +  
                        projectVersion.SourceControlVersion.Name + "/ " +  
                        projectVersion.Id + " - " + projectVersion.Project.Name + " - " + TaskRunnerContext.GetProjectVersionState(projectVersion.Id));
                }

                /*if (sourceControls.All(scv => TaskRunnerContext.GetSourceControlVersionState(scv.Id) == SourceControlState.Idle) &&
                    bundleVersions.All(bv => TaskRunnerContext.GetBundleVersionState(bv.Id) == BundleState.Idle))
                {
                    break;
                }*/

                if (WorkerComplete)
                {
                    break;
                }

                Thread.Sleep(1000);

                Console.Clear();
            }
        }
    }
}
