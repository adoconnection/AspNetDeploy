using System.Collections.Concurrent;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public class TaskRunnerContext2
    {
        private static readonly ConcurrentDictionary<int, BundleState> BundleStates = new ConcurrentDictionary<int, BundleState>();
        private static readonly ConcurrentDictionary<int, SourceControlState> SourceControlStates = new ConcurrentDictionary<int, SourceControlState>();
        private static readonly ConcurrentDictionary<int, ProjectState> ProjectStates = new ConcurrentDictionary<int, ProjectState>();

        public static SourceControlState GetSourceControlVersionState(int id)
        {
            return SourceControlStates.GetOrAdd(id, SourceControlState.Idle);
        }
        public static BundleState GetBundleVersionState(int id)
        {
            return BundleStates.GetOrAdd(id, BundleState.Idle);
        }

        
        public bool StartTakingVersion(int sourceControlVersionId, int bundleId)
        {
            SourceControlState sourceControlVersionState = GetSourceControlVersionState(sourceControlVersionId);
            BundleState bundleVersionState = GetBundleVersionState(bundleId);

            if (sourceControlVersionState == SourceControlState.Idle)
            {
                if (bundleVersionState == BundleState.Idle || bundleVersionState == BundleState.Loading)
                {
                    SourceControlStates.AddOrUpdate(sourceControlVersionId, SourceControlState.Loading, (i, state) => SourceControlState.Loading);
                    BundleStates.AddOrUpdate(bundleId, BundleState.Loading, (i, state) => BundleState.Loading);
                    return true;
                }
            }

            return false;
        }
    }
}