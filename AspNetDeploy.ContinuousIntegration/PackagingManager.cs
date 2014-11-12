using System.Linq;
using System.Xml.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Model;

namespace AspNetDeploy.ContinuousIntegration
{
    public class PackagingManager
    {
        private readonly IProjectPackagerFactory projectPackagerFactory;
        private readonly ISolutionParsersFactory solutionParsersFactory;

        public PackagingManager(IProjectPackagerFactory projectPackagerFactory, ISolutionParsersFactory solutionParsersFactory)
        {
            this.projectPackagerFactory = projectPackagerFactory;
            this.solutionParsersFactory = solutionParsersFactory;
        }

        public void Package(int projectVersionId)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();
            ProjectVersion projectVersion = entities.ProjectVersion
                .Include("SourceControlVersion.Properties")
                .Include("SourceControlVersion.SourceControl.Properties")
                .First( pv => pv.Id == projectVersionId);

            string sourcesFolder = string.Format(@"H:\AspNetDeployWorkingFolder\Sources\{0}\{1}", projectVersion.SourceControlVersion.SourceControl.Id, projectVersion.SourceControlVersion.Id);

            XDocument xDocument = XDocument.Load(projectFile);

            XNamespace fileNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        }
    }
}