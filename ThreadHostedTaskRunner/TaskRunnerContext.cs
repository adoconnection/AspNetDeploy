using System.Collections.Concurrent;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public static class TaskRunnerContext
    {
        private static readonly ConcurrentDictionary<int, SourceControlState> SourceControlStates = new ConcurrentDictionary<int, SourceControlState>();
        private static readonly ConcurrentDictionary<int, BundleState> BundleStates = new ConcurrentDictionary<int, BundleState>();
        private static readonly ConcurrentDictionary<int, ProjectState> ProjectStates = new ConcurrentDictionary<int, ProjectState>();

        private static readonly ConcurrentDictionary<int, bool> BuildProjects = new ConcurrentDictionary<int, bool>();

        public static bool NeedToBuildProject(int id)
        {
            return BuildProjects.GetOrAdd(id, false);
        }
        public static bool NeedToBuildProject(int id, bool value)
        {
            return BuildProjects.AddOrUpdate(id, value, (i, controlState) => value);
        }
        public static SourceControlState GetSourceControlState(int id)
        {
            return SourceControlStates.GetOrAdd(id, SourceControlState.Idle);
        }
        public static SourceControlState SetSourceControlState(int id, SourceControlState state)
        {
            return SourceControlStates.AddOrUpdate(id, state, (i, controlState) => state);
        }

        public static BundleState GetBundleState(int id)
        {
            return BundleStates.GetOrAdd(id, BundleState.Idle);
        }
        public static BundleState SetBundleState(int id, BundleState state)
        {
            return BundleStates.AddOrUpdate(id, state, (i, controlState) => state);
        }

        public static ProjectState GetProjectState(int id)
        {
            return ProjectStates.GetOrAdd(id, ProjectState.Idle);
        }
        public static ProjectState SetProjectState(int id, ProjectState state)
        {
            return ProjectStates.AddOrUpdate(id, state, (i, controlState) => state);
        }
    }
}