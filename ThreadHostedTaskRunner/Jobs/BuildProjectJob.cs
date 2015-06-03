using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner.Jobs
{
    public class BuildProjectJob
    {
        public void Start(int sourceControlVersionId, string solutionFileName, Action<int> projectBuildStarted, Action<int, bool> projectBuildComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            SourceControlVersion sourceControlVersion = entities.SourceControlVersion
                .Include("Properties")
                .First( scv => scv.Id == sourceControlVersionId);

            IDictionary<int, DateTime> buildTiming = new Dictionary<int, DateTime>();

            BuildManager buildManager = Factory.GetInstance<BuildManager>();
            buildManager.Build(
                sourceControlVersionId,
                solutionFileName,
                projectVersionId =>
                {
                    projectBuildStarted(projectVersionId);

                    if (buildTiming.ContainsKey(projectVersionId))
                    {
                        buildTiming.Remove(projectVersionId);
                    }

                    ProjectVersion projectVersion = entities.ProjectVersion
                        .Include("Properties")
                        .First(pv => pv.Id == projectVersionId);

                    projectVersion.SetStringProperty("LastBuildStartDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    entities.SaveChanges();

                    buildTiming[projectVersionId] = DateTime.UtcNow;

                },
                (projectVersionId, isSuccess) =>
                {
                    projectBuildComplete(projectVersionId, isSuccess);

                    ProjectVersion projectVersion = entities.ProjectVersion
                        .Include("Properties")
                        .First(pv => pv.Id == projectVersionId);

                    projectVersion.SetStringProperty("LastBuildRevision", sourceControlVersion.GetStringProperty("Revision"));
                    projectVersion.SetStringProperty("LastBuildDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    projectVersion.SetStringProperty("LastBuildResult", isSuccess.ToString());
                    projectVersion.SetStringProperty("LastBuildDuration", (DateTime.UtcNow - buildTiming[projectVersionId]).TotalSeconds.ToString(CultureInfo.InvariantCulture));

                    entities.SaveChanges();
                });



            entities.SaveChanges();
        }

    }
}