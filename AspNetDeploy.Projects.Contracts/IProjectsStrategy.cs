using System;
using System.Collections.Generic;
using AspNetDeploy.Model;

namespace AspNetDeploy.Projects.Contracts
{
    public interface IProjectsStrategy
    {
        void LoadProjects();
        void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid);
        bool IsExists(Guid guid);
        IList<Guid> ListProjectGuids();
        void UpdateProject(Project project, Guid guid);
    }
}