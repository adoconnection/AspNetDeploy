using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ITaskRunner
    {
        void Initialize();

        void WatchForSources();

        SourceControlState GetSourceControlState(int sourceControlId);

        ProjectState GetProjectState(int projectId);

        BundleState GetBundleState(int bundleId);
        MachineState GetMachineState(int machineId);

        void Shutdown();
    }
}