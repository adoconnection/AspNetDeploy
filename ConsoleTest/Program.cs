using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using AspNetDeploy.WebUI.Bootstrapper;
using ObjectFactory;
using ThreadHostedTaskRunner;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            ThreadTaskRunner.ProcessTasks();

            AspNetDeployEntities entities = new AspNetDeployEntities();

            while (true)
            {
                Console.WriteLine(DateTime.Now);
                List<SourceControlVersion> sourceControls = entities.SourceControlVersion.Include("SourceControl").ToList();

                foreach (SourceControlVersion sourceControlVersion in sourceControls)
                {
                    Console.WriteLine(sourceControlVersion.Id + " - " + sourceControlVersion.SourceControl.Name + " / " + sourceControlVersion.Name + " - " + TaskRunnerContext.GetSourceControlVersionState(sourceControlVersion.Id));
                }

                if (sourceControls.All(sc => TaskRunnerContext.GetSourceControlVersionState(sc.Id) == SourceControlState.Idle))
                {
                    break;
                }

                Thread.Sleep(500);

                Console.Clear();
                
            }

            Console.WriteLine("Complete");
            Console.ReadKey();
        }
    }
}
