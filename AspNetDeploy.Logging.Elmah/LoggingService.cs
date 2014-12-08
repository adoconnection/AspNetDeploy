using System;
using System.Web;
using AspNetDeploy.Contracts;
using Elmah;

namespace AspNetDeploy.Logging.ElmahSvc
{
    public class LoggingService : ILoggingService
    {
        public void Log(Exception exception)
        {

            Elmah.ErrorLog.GetDefault(null).Log(new Error(exception));
        }
    }
}
