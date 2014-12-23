using System;
using AspNetDeploy.Contracts;
using Elmah;

namespace AspNetDeploy.Logging.ElmahSvc
{
    public class ElmahLoggingService : ILoggingService
    {
        public void Log(Exception exception, int? userId)
        {

            ErrorLog.GetDefault(null).Log(new Error(exception));
        }
    }
}
