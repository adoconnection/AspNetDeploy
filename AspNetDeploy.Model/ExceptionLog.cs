//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AspNetDeploy.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExceptionLog
    {
        public ExceptionLog()
        {
            this.AspNetDeployException = new HashSet<AspNetDeployExceptionLog>();
            this.ExceptionData = new HashSet<ExceptionLogData>();
            this.MachinePublicationLog = new HashSet<MachinePublicationLog>();
            this.ParentExceptions = new HashSet<ExceptionLog>();
        }
    
        public int Id { get; set; }
        public int InnerExceptionId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string TypeName { get; set; }
    
        public virtual ICollection<AspNetDeployExceptionLog> AspNetDeployException { get; set; }
        public virtual ICollection<ExceptionLogData> ExceptionData { get; set; }
        public virtual ICollection<MachinePublicationLog> MachinePublicationLog { get; set; }
        public virtual ICollection<ExceptionLog> ParentExceptions { get; set; }
        public virtual ExceptionLog InnerException { get; set; }
    }
}
