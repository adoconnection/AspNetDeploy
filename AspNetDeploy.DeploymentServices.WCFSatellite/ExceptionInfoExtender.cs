using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Contracts.Exceptions;

namespace AspNetDeploy.DeploymentServices.WCFSatellite.DeploymentServiceReference
{
    public partial class ExceptionInfo : IExceptionInfo
    {
        IList<IExceptionDataInfo> IExceptionInfo.ExceptionData
        {
            get
            {
                return this.ExceptionData.Cast<IExceptionDataInfo>().ToList();
            }
        }

        IExceptionInfo IExceptionInfo.InnerException
        {
            get
            {
                return this.InnerException;
            }
        }
    }
}