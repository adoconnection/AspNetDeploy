using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AspNetDeploy.BuildServices.DotnetCore;
using AspNetDeploy.BuildServices.MSBuild;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.MachineSummary;
using AspNetDeploy.DeploymentServices;
using AspNetDeploy.DeploymentServices.SatelliteMonitoring;
using AspNetDeploy.DeploymentServices.WCFSatellite;
using AspNetDeploy.Model;
using AspNetDeploy.SolutionParsers.VisualStudio;
using AspNetDeploy.Variables;
using AspNetDeploy.WebUI.Bootstrapper;
using BuildServices.NuGet;
using LocalEnvironment;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using ObjectFactory;
using ThreadHostedTaskRunner;
using Microsoft.Build.Construction;
using Microsoft.Web.Administration;
using Newtonsoft.Json;
using Packagers.Gulp;
using Packagers.VisualStudioProject;
using Projects.Gulp;
using SatelliteService.Operations;
using TestRunners;
using VsTestLibrary;
using Environment = AspNetDeploy.Model.Environment;

namespace ConsoleTest
{
    public class ConsoleLogger : ILogger
    {
        public void Initialize(IEventSource eventSource)
        {
            //eventSource.ProjectStarted += (sender, args) => Console.WriteLine("Started " + args.ProjectFile);
            eventSource.ProjectFinished += (sender, args) => Console.WriteLine("finished " + args.ProjectFile);
            eventSource.ErrorRaised += (sender, args) =>
            {
                Console.WriteLine("ERROR");
                Console.WriteLine("* " + args.ProjectFile);
                Console.WriteLine("* " + args.File);
                Console.WriteLine("* " + args.LineNumber + " / " + args.ColumnNumber);
                Console.WriteLine("* " + args.Message);
            };
        }

        public void Shutdown()
        {

        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }

    class Program
    {



        private static bool WorkerComplete = false;



        static void Main(string[] args)
        {
            var variableProcessor = new VariableProcessor(null, null, null);
            WCFSatelliteDeploymentAgent agent = new WCFSatelliteDeploymentAgent(variableProcessor, "https://machine84.limetime.io:8090/AspNetDeploySatellite", "pl9uJLX94jieM32SID5b8axMJE2LpD04", "L2l3lp5LnXfIRcaaduABlOeF45iPtTod");

            var isReady = agent.IsReady();


            XDocument webConfig = XDocument.Load(@"H:\Documentoved\FiasOnline\FiasOnline.UI.API\bin\Release\netcoreapp2.0\publish\web.config");

            if (webConfig.Descendants("aspNetCore").Any(dd => dd.Attribute("processPath")?.Value == "dotnet"))
            {
            }

            using (ServerManager serverManager = new ServerManager())
            {
                //this.backupSiteConfigurationGuid = this.BackupRepository.StoreObject(site);

                ApplicationPool applicationPool = ApplicationPool(serverManager, "AspNetDeploy");
                ApplicationPool applicationPool2 = ApplicationPool(serverManager, "ButtonBackend1");
            }



            VisualStudio2013SolutionParser solutionParser = new VisualStudio2013SolutionParser();
            IList<VisualStudioSolutionProject> projects = solutionParser.Parse(@"H:\Documentoved\FiasOnline\FiasOnline.sln");

            foreach (VisualStudioSolutionProject project in projects)
            {
                Console.WriteLine(project.Name + " - " + project.Type + " - " + project.Guid + " - " + project.TypeGuid);
            }

            DotnetCoreBuildService buildService = new DotnetCoreBuildService();

            ProjectVersion pv = new ProjectVersion()
            {
                Name = "Blah",
                ProjectType = ProjectType.Web | ProjectType.NetCore,
                ProjectFile = @"FiasOnline.UI.API\FiasOnline.UI.API.csproj",
                SolutionFile = @"H:\Documentoved\FiasOnline"
            };

            buildService.Build(@"H:\Documentoved\FiasOnline", pv, s =>
            {
            }, (s, b, arg3) =>
            {
            }, (s, exception) =>
            {
            });

            if (File.Exists(@"H:\Documentoved\FiasOnline\blah.zip"))
            {
                File.Delete(@"H:\Documentoved\FiasOnline\blah.zip");
            }

            DotNetCoreProjectPackager projectPackager = new DotNetCoreProjectPackager();
            projectPackager.Package(@"H:\Documentoved\FiasOnline\FiasOnline.UI.API\FiasOnline.UI.API.csproj", @"H:\Documentoved\FiasOnline\blah.zip");

            Console.WriteLine(projects.Count);



            //ThreadTaskRunner.ProcessTasks();

            /*
            VariableProcessorFactory variableProcessorFactory = new VariableProcessorFactory();
            ProjectTestRunnerFactory projectTestRunnerFactory = new ProjectTestRunnerFactory(new PathServices());

            IVariableProcessor variableProcessor = variableProcessorFactory.Create(278, 9);

            AspNetDeployEntities entities = new AspNetDeployEntities();
            ProjectVersion projectVersion2 = entities.ProjectVersion.Include("SourceControlVersion").First( v => v.Id == 14746);


            IProjectTestRunner projectTestRunner = projectTestRunnerFactory.Create(projectVersion2.ProjectType, variableProcessor);

            IList<TestResult> testResults = projectTestRunner.Run(projectVersion2);

          */

            VsTestParser parser = new VsTestParser();
            IList<VsTestClassInfo> testClasses = parser.Parse(@"H:\Documentoved\DocumentovedUITests\AccountTests\bin\Debug\AccountTests.dll");

            IList<TestResult> result = new List<TestResult>();

            IDictionary<string, string> d = new ConcurrentDictionary<string, string>();

            string c = null;

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine(c);
                    foreach (string dKey in d.Keys)
                    {
                        Console.WriteLine(" - " + dKey + " - " + d[dKey]);
                    }

                    Thread.Sleep(1000);
                }
            });

            t.Start();

            foreach (VsTestClassInfo testClass in testClasses)
            {


                c = testClass.Type.FullName;
                d.Clear();

                Parallel.ForEach(testClass.TestMethods, new ParallelOptions
                {
                    MaxDegreeOfParallelism = 5
                }, testMethod =>
                {
                    d.Add(testMethod, "started");

                    VsTestRunner runner = new VsTestRunner();
                    VsTestRunResult vsTestRunResult = runner.Run(testClass.Type, testClass.InitializeMethod, testMethod, testClass.CleanupMethod);

                    result.Add(new TestResult()
                    {
                        TestClassName = testClass.Type.FullName,
                        TestName = testMethod,
                        IsPass = vsTestRunResult.IsSuccess,
                        Message = vsTestRunResult.Exception?.Message + ". " + vsTestRunResult.Exception?.InnerException?.Message
                    });

                    d[testMethod] = vsTestRunResult.IsSuccess ? "done" : "fail";
                });
            }



            Console.WriteLine("DONE");
            Console.ReadKey();


            /*
            GulpParser gulpParser = new GulpParser(@"H:\Documentoved\Resources");

            gulpParser.LoadProjects();

            GulpProjectPackager gulpProjectPackager = new GulpProjectPackager();

            gulpProjectPackager.Package(@"H:\Documentoved\Resources\documentoved.gulpfile.js", @"H:\Documentoved\Resources\result.zip");


            return;
            */
            /*
            dynamic bindingConfig = JsonConvert.DeserializeObject("{ port:80, host:'abc.local'}");

            using (ServerManager serverManager = new ServerManager())
            {
                Site site = serverManager.Sites["Account Service Latest"];

                foreach (Binding siteBinding in site.Bindings)
                {
                    siteBinding.BindingInformation = (string.IsNullOrWhiteSpace((string)bindingConfig.IP) ? "" : (string)bindingConfig.IP) + ":" + (int)bindingConfig.port + ":" + (string)bindingConfig.host;
                }

                serverManager.CommitChanges();
            }

            return;
            */


            /*

                        Machine machine = entities.Machine.First( m => m.Name == "Lake");

                        WCFSatelliteDeploymentAgent agent = new WCFSatelliteDeploymentAgent(null, machine.URL, machine.Login, machine.Password, new TimeSpan(0, 0, 0, 1), new TimeSpan(0, 0, 0, 1));

                        Console.WriteLine("Runnning");
                        Console.WriteLine(agent.IsReady());
            */

            /*

                         List<Machine> machines = entities.Machine.ToList();

                         machines.Where( m => !m.Name.Contains("FastVPS") && !m.Name.Contains("Snow")).ToList().AsParallel().ForAll(machine =>
                         {
                             WCFSatelliteDeploymentAgent deploymentAgent = new WCFSatelliteDeploymentAgent(null, machine.URL, machine.Login, machine.Password, new TimeSpan(0, 0, 1));
                             Console.WriteLine(machine.Id + " - " + machine.Name + " - " + deploymentAgent.IsReady());
                         });

                        Console.WriteLine("-=====================-");

                        ISatelliteMonitor satelliteMonitor = new SatelliteMonitor();

                        List<Environment> environments = entities.Environment
                            .Include("Properties")
                            .Include("Machines.MachineRoles")
                            .ToList();

                        Dictionary<Machine, SatelliteState> dictionary = environments.SelectMany(e => e.Machines)
                            .Distinct()
                            .AsParallel()
                            .Select(m => new { m, alive = satelliteMonitor.IsAlive(m) })
                            .ToDictionary(k => k.m, k => k.alive);

                        DateTime start = DateTime.Now;

                        Dictionary<Machine, IServerSummary> summaries = environments.SelectMany(e => e.Machines)
                            .Distinct()
                            .AsParallel()
                            .Select(m => new { m, summary = satelliteMonitor.GetServerSummary(m) })
                            .ToDictionary(k => k.m, k => k.summary);

                        DateTime end = DateTime.Now;

                        Console.WriteLine((end-start).TotalSeconds);

                        foreach (KeyValuePair<Machine, SatelliteState> serverSummary in dictionary)
                        {
                            Console.WriteLine(serverSummary.Key + " - " + serverSummary.Value);
                        }
            */


            MSBuildBuildService buildBuildService = new MSBuildBuildService(new PathServices());

            DateTime? startDate = null;

            ProjectVersion projectVersion = new ProjectVersion()
            {
                // ProjectFile = @"ZelbikeRace2Database\ZelbikeRace2Database.sqlproj",
                ProjectFile = @"Services.Accounts\Services.Accounts.csproj",
                SolutionFile = @"Documentoved.sln"
            };

            Console.WriteLine("Starting");

            BuildSolutionResult buildSolutionResult = buildBuildService.Build(@"C:\AspNetDeployWorkingFolderO\Sources\5\207", projectVersion,
                s =>
                {
                    if (startDate == null)
                    {
                        startDate = DateTime.Now;
                    }

                    Console.WriteLine(s + " - started");
                },
                (s, b, arg3) =>
                {
                    Console.WriteLine(s + " - " + b);
                },
                (s, s1) =>
                {
                    // e.ProjectFile, e.File, e.Code, e.LineNumber, e.ColumnNumber, e.Message
                    Console.WriteLine(s + "\n" + s1);
                    Console.WriteLine(s1.Message + "\n" + s1);
                });

            DateTime endDate = DateTime.Now;

            Console.WriteLine(buildSolutionResult.IsSuccess);
            Console.WriteLine((endDate - startDate.Value).TotalMilliseconds);



            /*
            //string path = @"H:\Documentoved\Latest\Services.ImsPrimary\Databases.ImsPrimary.sqlproj";
            string path = @"H:\Documentoved\Latest\Services.ExceptionHandlingService\Services.Logging.csproj";

            //  SolutionFile solutionFile = SolutionFile.Parse(path);

            // ProjectRootElement element = ProjectRootElement.Open(@"H:\Documentoved\Latest\Services.ExceptionHandlingService\Services.Logging.csproj");


            Dictionary<string, string> globalProperty = new Dictionary<string, string>
            {
                {"Configuration", "Release"}
            };

            ProjectCollection projectCollection = new ProjectCollection();

            BuildRequestData buildRequestData = new BuildRequestData(path, globalProperty, "14.0", new[] { "Rebuild" }, null);

            BuildParameters buildParameters = new BuildParameters(projectCollection);
            buildParameters.MaxNodeCount = 1;
            buildParameters.Loggers = new List<ILogger>
            {
                new ConsoleLogger()
            };




            BuildResult buildResult = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

           */

            //Console.WriteLine("building " + buildResult.OverallResult);

            /*

                        ObjectFactoryConfigurator.Configure();

                        Console.WriteLine("Testing satellites");

                        AspNetDeployEntities entities = new AspNetDeployEntities();

                        MSBuildBuildService service = new MSBuildBuildService(new NugetPackageManager(new PathServices()));
                        service.Build(
                            @"H:\AspNetDeployWorkingFolder\Sources\5\45\CodeBase.Documents.WebUI\CodeBase.Documents.WebUI.csproj",
                            s => Console.WriteLine("started: " + s), (s, b, arg3) => Console.WriteLine("done: " + arg3), (s, s1, arg3, arg4, arg5, arg6) => Console.WriteLine("Err: " + s));*/

            /*foreach (Machine machine in entities.Machine)
            {
                Console.Write(machine.Name + "...");
                Console.WriteLine(Factory.GetInstance<ISatelliteMonitor>().IsAlive(machine));
            }

            Console.ReadKey();

            RunScheduler();*/

            WriteLine("Main thread complete");
            Console.ReadKey();
        }

        private static void RunScheduler()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, eventArgs) => ThreadTaskRunner.ProcessTasks();
            worker.RunWorkerCompleted += (sender, eventArgs) =>
            {
                WriteLine("---------------");
                WriteLine("Worker Complete");
                WorkerComplete = true;
            };
            worker.RunWorkerAsync();

            AspNetDeployEntities entities = new AspNetDeployEntities();

            while (true)
            {
                WriteLine(DateTime.Now.ToString());
                List<SourceControlVersion> sourceControls = entities.SourceControlVersion.Include("SourceControl").ToList();
                List<BundleVersion> bundleVersions = entities.BundleVersion.Include("Bundle").ToList();
                List<ProjectVersion> projectVersions = entities.ProjectVersion.Include("Project").ToList();
                List<Machine> machines = entities.Machine.ToList();

                foreach (SourceControlVersion sourceControlVersion in sourceControls)
                {
                    WriteLine(sourceControlVersion.Id + " - " + sourceControlVersion.SourceControl.Name + " / " + sourceControlVersion.Name + " - " + TaskRunnerContext.GetSourceControlVersionState(sourceControlVersion.Id));
                }

                WriteLine("");

                foreach (BundleVersion bundleVersion in bundleVersions)
                {
                    WriteLine(bundleVersion.Id + " - " + bundleVersion.Bundle.Name + " / " + bundleVersion.Name + " - " + TaskRunnerContext.GetBundleVersionState(bundleVersion.Id));
                }

                WriteLine("");

                foreach (ProjectVersion projectVersion in projectVersions.Where(p => TaskRunnerContext.GetProjectVersionState(p.Id) != ProjectState.Idle))
                {
                    WriteLine(
                        projectVersion.SourceControlVersion.SourceControl.Name + "/ " +
                        projectVersion.SourceControlVersion.Name + "/ " +
                        projectVersion.Id + " - " + projectVersion.Project.Name + " - " + TaskRunnerContext.GetProjectVersionState(projectVersion.Id));
                }

                WriteLine("");

                foreach (Machine machine in machines)
                {
                    WriteLine(
                        machine.Name + " - " + TaskRunnerContext.GetMachineState(machine.Id));
                }

                WriteLine("");

                /*if (sourceControls.All(scv => TaskRunnerContext.GetSourceControlVersionState(scv.Id) == SourceControlState.Idle) &&
                    bundleVersions.All(bv => TaskRunnerContext.GetBundleVersionState(bv.Id) == BundleState.Idle))
                {
                    break;
                }*/

                if (WorkerComplete)
                {
                    break;
                }

                Thread.Sleep(200);

                Console.SetCursorPosition(0, 0);
            }
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth - 1) + "\r" + value);
        }


        private static Site Site(ServerManager serverManager, string siteName)
        {
            Site site = serverManager.Sites[siteName];

            if (site == null)
            {
                long nextId = serverManager.Sites.Count == 0
                    ? 1
                    : serverManager.Sites.Max(s => s.Id) + 1;

                site = serverManager.Sites.CreateElement();
                site.Id = nextId;
                site.Name = siteName;
                serverManager.Sites.Add(site);
            }

            return site;
        }


        private static ApplicationPool ApplicationPool(ServerManager serverManager, string applicationPoolName)
        {
            ApplicationPool applicationPool = serverManager.ApplicationPools[applicationPoolName];

            if (applicationPool == null)
            {
                applicationPool = serverManager.ApplicationPools.Add(applicationPoolName);
            }

            return applicationPool;
        }
    }
}
