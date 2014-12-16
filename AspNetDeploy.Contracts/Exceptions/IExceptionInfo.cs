using System.Collections.Generic;

namespace AspNetDeploy.Contracts.Exceptions
{
    public interface IExceptionInfo
    {
        string AssemblyQualifiedTypeName { get; }

        IList<IExceptionDataInfo> ExceptionData { get; }

        IExceptionInfo InnerException { get; }

        string Message { get; }

        string Source { get; }

        string StackTrace { get; }

        string TypeName { get; }
    }
}