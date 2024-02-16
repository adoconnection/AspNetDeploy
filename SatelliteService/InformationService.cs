using System.ServiceModel;

namespace SatelliteService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class InformationService : IInformationService
    {
        public int GetVersion()
        {
            return 20240216;
        }
    }
}