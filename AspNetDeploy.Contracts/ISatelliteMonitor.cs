using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ISatelliteMonitor
    {
        SatelliteState IsAlive(Machine machine);
    }
}