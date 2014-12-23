using System;
using AspNetDeploy.Contracts.Exceptions;
using AspNetDeploy.Model;

namespace AspNetDeploy.Contracts
{
    public interface ILoggingService
    {
        void Log(Exception exception, int? userId);
    }
}