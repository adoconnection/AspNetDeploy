using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ObjectFactory;
using SatelliteService.Bootstrapper;
using SatelliteService.Contracts;
using SatelliteService.Operations;
using SatelliteService.Packages;

namespace SatelliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ObjectFactoryConfigurator.Configure();

            IPathRepository pathRepository = Factory.GetInstance<IPathRepository>();
            IBackupRepository backupRepository = Factory.GetInstance<IBackupRepository>();

            /*  IBackupRepository backupRepository = Factory.GetInstance<IBackupRepository>();

            backupRepository.RestoreDirectory(Guid.Parse("4487e739-5d2c-44b2-bf2a-a01692b60de0"));

            Guid storeDirectory = backupRepository.StoreFile(@"H:\AspNetDeploySatelliteWorkingFolder\TestFolder\Project15\Web.config");

            Console.WriteLine("STORED = " + storeDirectory);
            Console.ReadKey();

            backupRepository.RestoreFile(storeDirectory);

            Console.WriteLine("RESTORED");
            Console.ReadKey();
*/

            /*ConfigOperation configOperation = new ConfigOperation(backupRepository);

            configOperation.Configure(JsonConvert.DeserializeObject(@"
            {
                file : 'H:\\AspNetDeploySatelliteWorkingFolder\\TestFolder\\Project15\\Web.config',
                content : '
                    <configuration>
                        <system.web>
                            <compilation debug=""false"" />
                        </system.web>
                        <appSettings childNodesKeyName=""key"">
                            <add key=""Site.Version"" value=""1.2"" />
                            <add key=""BackgroundCMS.TargetAuthorizeUrl"" value=""{var:BackgroundCMS.TargetAuthorizeUrl}"" />
                        </appSettings>
                    </configuration>
                '
            }"), new Dictionary<string, object>
            {
                {"BackgroundCMS.TargetAuthorizeUrl", "http://omg.ru"}
            });*/

            UpdateHostsOperation operation = new UpdateHostsOperation(backupRepository);
            operation.Configure(JsonConvert.DeserializeObject(@"
            {
                add : [
                    {ip: '127.0.0.1', domain: 'testing-1.local'},
                    {ip: '127.0.0.2', domain: 'testing-2.local'}
                ]
            }"));

            operation.Run();
            Console.WriteLine("UPDATED");
            Console.ReadKey();

            operation.Rollback();
            Console.WriteLine("REVERTED");
            Console.ReadKey();

            /*WebSiteOperation webSiteOperation = new WebSiteOperation(backupRepository, new PackageRepository(pathRepository.GetPackagePath(6)));


            webSiteOperation.Configure(JsonConvert.DeserializeObject(@"
            {
                destination : 'H:\\AspNetDeploySatelliteWorkingFolder\\TestFolder\\Project15',
                siteName : 'Project 15',
                applicationPoolName : 'Project 15',
                projectId : 15,
                bindings : [
                
                    {
                        port : 80,
                        protocol : 'http',
                        url : 'project15.local'
                    }
                ]
            }"));

            webSiteOperation.Run();
            webSiteOperation.Rollback();*/



            /*if (Directory.Exists(@"H:\AspNetDeploySatelliteWorkingFolder\TestFolder"))
            {
                Directory.Delete(@"H:\AspNetDeploySatelliteWorkingFolder\TestFolder", true);
                Directory.CreateDirectory(@"H:\AspNetDeploySatelliteWorkingFolder\TestFolder");
            }
            */
            /*IPathRepository pathRepository = Factory.GetInstance<IPathRepository>();*/

            /*PackageRepository packageRepository = new PackageRepository(pathRepository.GetPackagePath(6));
            packageRepository.ExtractProject(15, @"H:\AspNetDeploySatelliteWorkingFolder\TestFolder\Project15");
            packageRepository.ExtractProject(19, @"H:\AspNetDeploySatelliteWorkingFolder\TestFolder\Project19");
*/

            Console.WriteLine("Complete");
            Console.ReadKey();
        }
    }
}
