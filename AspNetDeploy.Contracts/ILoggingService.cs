using System;

namespace AspNetDeploy.Contracts
{
    public interface ILoggingService
    {
        void Log(Exception exception);
    }
}