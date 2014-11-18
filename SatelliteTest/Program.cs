using System;
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

            WebSiteOperation webSiteOperation = new WebSiteOperation(backupRepository, new PackageRepository(pathRepository.GetPackagePath(6)));


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
