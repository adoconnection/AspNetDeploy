using System;

namespace AspNetDeploy.Projects
{
    public abstract class ProjectsStrategy
    {
        public abstract void LoadProjects();
        public abstract void UpdateProjectVersion(ProjectVersion projectVersion, Guid guid);
        public abstract bool IsExists(Guid guid);
    }
}