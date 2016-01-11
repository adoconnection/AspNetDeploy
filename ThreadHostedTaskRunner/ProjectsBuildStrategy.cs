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
    public class ProjectsBuildStrategy
    {
        private readonly AspNetDeployEntities entities;

        public ProjectsBuildStrategy(AspNetDeployEntities entities)
        {
            this.entities = entities;
        }

        public void Build(List<ProjectVersion> projectVersions)
        {
            ProjectVersion activeProjectVersion = null;

            try
            {
                foreach (ProjectVersion projectVersion in projectVersions.Distinct())
                {
                    activeProjectVersion = projectVersion;

                    projectVersion.SetStringProperty("LastBuildResult", "Not started");
                    entities.SaveChanges();

                    BuildJob job = new BuildJob();
                    job.Start(
                        projectVersion.SourceControlVersion.Id,
                        projectVersion.ProjectFile,
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
            }
            catch (Exception e)
            {
                AspNetDeployException aspNetDeployException = new AspNetDeployException("Build project failed", e);
                aspNetDeployException.Data.Add("SourceControl version Id", activeProjectVersion.SourceControlVersion.Id);
                aspNetDeployException.Data.Add("Solution file", activeProjectVersion.SolutionFile);

                Factory.GetInstance<ILoggingService>().Log(aspNetDeployException, null);

                if (e.IsCritical())
                {
                    throw;
                }
            }
        }
    }
}