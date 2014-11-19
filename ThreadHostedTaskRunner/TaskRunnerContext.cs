using System.Collections.Concurrent;
using System.Collections.Generic;
using AspNetDeploy.Model;

namespace ThreadHostedTaskRunner
{
    public static class TaskRunnerContext
    {
        private static readonly object LockObject = new object();

        private static readonly IDictionary<int, List<int>> SourceControlLocks = new Dictionary<int, List<int>>();
        private static readonly IDictionary<int, List<int>> BundleLocks = new Dictionary<int, List<int>>();

        private static readonly ConcurrentDictionary<int, BundleState> BundleStates = new ConcurrentDictionary<int, BundleState>();
        private static readonly ConcurrentDictionary<int, SourceControlState> SourceControlStates = new ConcurrentDictionary<int, SourceControlState>();
        private static readonly ConcurrentDictionary<int, ProjectState> ProjectStates = new ConcurrentDictionary<int, ProjectState>();
        private static readonly ConcurrentDictionary<int, MachineState> MachineStates = new ConcurrentDictionary<int, MachineState>();

        public static void LockSourceControl(int sourceControlVersionId, int bundleId)
        {
            AddLock(SourceControlLocks, sourceControlVersionId, bundleId);
        }
        public static void UnLockSourceControl(int sourceControlVersionId, int bundleId)
        {
            RemoveLock(SourceControlLocks, sourceControlVersionId, bundleId);
        }
        public static void IsSourceControlLocked(int sourceControlVersionId)
        {
            IsLocked(SourceControlLocks, sourceControlVersionId);
        }

        public static void LockBundleControl(int bundleId, int sourceControlVersionId)
        {
            AddLock(BundleLocks, bundleId, sourceControlVersionId);
        }
        public static void UnLockBundleControl(int bundleId, int sourceControlVersionId)
        {
            RemoveLock(BundleLocks, bundleId, sourceControlVersionId);
        }
        public static void IsBundleLocked(int bundleId)
        {
            IsLocked(BundleLocks, bundleId);
        }

        private static void AddLock(IDictionary<int, List<int>> source, int lockedObject, int locker)
        {
            lock (LockObject)
            {
                if (!source.ContainsKey(lockedObject))
                {
                    source.Add(lockedObject, new List<int>());
                }

                List<int> lockers = source[lockedObject];

                if (!lockers.Contains(locker))
                {
                    lockers.Add(locker);
                }
            }
        }

        private static void RemoveLock(IDictionary<int, List<int>> source, int lockedObject, int locker)
        {
            lock (LockObject)
            {
                if (!source.ContainsKey(lockedObject))
                {
                    source.Add(lockedObject, new List<int>());
                }

                List<int> lockers = source[lockedObject];

                if (lockers.Contains(locker))
                {
                    lockers.Remove(locker);
                }
            }
        }

        private static bool IsLocked(IDictionary<int, List<int>> source, int lockedObject)
        {
            lock (LockObject)
            {
                if (!source.ContainsKey(lockedObject))
                {
                    source.Add(lockedObject, new List<int>());
                }

                return source[lockedObject].Count == 0;
            }
        }

        public static SourceControlState GetSourceControlVersionState(int id)
        {
            return SourceControlStates.GetOrAdd(id, SourceControlState.Idle);
        }
        public static SourceControlState SetSourceControlVersionState(int id, SourceControlState state)
        {
            return SourceControlStates.AddOrUpdate(id, state, (i, s) => state);
        }

        public static BundleState GetBundleVersionState(int id)
        {
            return BundleStates.GetOrAdd(id, BundleState.Idle);
        }
        public static BundleState SetBundleVersionState(int id, BundleState state)
        {
            return BundleStates.AddOrUpdate(id, state, (i, s) => state);
        }

        public static ProjectState GetProjectVersionState(int id)
        {
            return ProjectStates.GetOrAdd(id, ProjectState.Idle);
        }
        public static ProjectState SetProjectVersionState(int id, ProjectState state)
        {
            return ProjectStates.AddOrUpdate(id, state, (i, s) => state);
        }

        public static MachineState GetMachineState(int id)
        {
            return MachineStates.GetOrAdd(id, MachineState.Idle);
        }
        public static MachineState SetMachineState(int id, MachineState state)
        {
            return MachineStates.AddOrUpdate(id, state, (i, s) => state);
        }

    }
}