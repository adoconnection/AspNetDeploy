using System.Collections.Generic;
using AspNetDeploy.Contracts.Exceptions;

namespace AspNetDeploy.ContinuousIntegration
{
    public class ExceptionInfo : IExceptionInfo
    {
        public string AssemblyQualifiedTypeName { get; set; }
        public IList<IExceptionDataInfo> ExceptionData { get; set; }
        public IExceptionInfo InnerException { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string TypeName { get; set; }
    }

    public class ExceptionDataInfo : IExceptionDataInfo
    {
        public bool IsProperty { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}