using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AspNetDeploy.ContinuousIntegration;
using AspNetDeploy.Model;
using ObjectFactory;

namespace ThreadHostedTaskRunner.Jobs
{
    public class BuildJob
    {
        public void Start(int projectVersionId, Action<int> projectBuildStarted, Action<int, bool> projectBuildComplete)
        {
            AspNetDeployEntities entities = new AspNetDeployEntities();

            ProjectVersion projectVersion = entities.ProjectVersion
                .Include("Properties")
                .First(pv => pv.Id == projectVersionId);


            SourceControlVersion sourceControlVersion = entities.SourceControlVersion
                .Include("Properties")
                .First( scv => scv.Id == projectVersion.SourceControlVersionId);

            IDictionary<int, DateTime> buildTiming = new Dictionary<int, DateTime>();

            BuildManager buildManager = Factory.GetInstance<BuildManager>();
            buildManager.Build(
                sourceControlVersion.Id,
                projectVersionId,
                projectVersionBuildId =>
                {
                    projectBuildStarted(projectVersionBuildId);

                    if (buildTiming.ContainsKey(projectVersionBuildId))
                    {
                        buildTiming.Remove(projectVersionBuildId);
                    }

                    ProjectVersion projectVersionBuild = entities.ProjectVersion
                        .Include("Properties")
                        .First(pv => pv.Id == projectVersionBuildId);

                    projectVersionBuild.SetStringProperty("LastBuildStartDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    entities.SaveChanges();

                    buildTiming[projectVersionBuildId] = DateTime.UtcNow;

                },
                (projectVersionBuildId, isSuccess) =>
                {
                    projectBuildComplete(projectVersionBuildId, isSuccess);

                    ProjectVersion projectVersionBuild = entities.ProjectVersion
                        .Include("Properties")
                        .First(pv => pv.Id == projectVersionBuildId);

                    projectVersionBuild.SetStringProperty("LastBuildRevision", sourceControlVersion.GetStringProperty("Revision"));
                    projectVersionBuild.SetStringProperty("LastBuildDate", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                    projectVersionBuild.SetStringProperty("LastBuildResult", isSuccess.ToString());
                    projectVersionBuild.SetStringProperty("LastBuildDuration", (DateTime.UtcNow - buildTiming[projectVersionBuildId]).TotalSeconds.ToString(CultureInfo.InvariantCulture));

                    entities.SaveChanges();
                });



            entities.SaveChanges();
        }

    }
}