using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;
using Exceptions;
using ObjectFactory;
using ThreadHostedTaskRunner.Jobs;

namespace ThreadHostedTaskRunner
{
    public class SolutionBuildStrategy
    {
        public void Build(AspNetDeployEntities entities, List<ProjectVersion> projectVersions)
        {
            foreach (var grouping in projectVersions.GroupBy(pv => new { pv.SolutionFile, pv.SourceControlVersion }))
            {
                try
                {
                    foreach (ProjectVersion projectVersion in grouping)
                    {
                        projectVersion.SetStringProperty("LastBuildResult", "Not started");
                        entities.SaveChanges();
                    }

                    BuildJob job = new BuildJob();
                    job.Start(
                        grouping.Key.SourceControlVersion.Id,
                        grouping.Key.SolutionFile,
                        projectId => TaskRunnerContext.SetProjectVersionState(projectId, ProjectState.Building),
                        (projectId, isSuccess) =>
                        {
                            ProjectVersion handlerProjectVersion = projectVersions.FirstOrDefault(pv => pv.Id == projectId);

                            if (handlerProjectVersion != null)
                            {
                                handlerProjectVersion.SetStringProperty("LastBuildResult", isSuccess ? "Done" : "Error");
                                entities.SaveChanges();
                            }

                            TaskRunnerContext.SetProjectVersionState(projectId, isSuccess ? ProjectState.Idle : ProjectState.Error);
                        });
                }
                catch (Exception e)
                {
                    AspNetDeployException aspNetDeployException = new AspNetDeployException("Build project failed", e);
                    aspNetDeployException.Data.Add("SourceControl version Id", grouping.Key.SourceControlVersion.Id);
                    aspNetDeployException.Data.Add("Solution file", grouping.Key.SolutionFile);

                    Factory.GetInstance<ILoggingService>().Log(aspNetDeployException, null);

                    if (e.IsCritical())
                    {
                        throw;
                    }
                }
            }
        } 
    }
}