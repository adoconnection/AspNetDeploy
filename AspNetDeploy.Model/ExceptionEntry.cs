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
    
    public partial class ExceptionEntry
    {
        public ExceptionEntry()
        {
            this.AspNetDeployException = new HashSet<AspNetDeployExceptionEntry>();
            this.ParentExceptionEntry = new HashSet<ExceptionEntry>();
            this.ExceptionData = new HashSet<ExceptionEntryData>();
            this.MachinePublicationLog = new HashSet<MachinePublicationLog>();
        }
    
        public int Id { get; set; }
        public Nullable<int> InnerExceptionId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string TypeName { get; set; }
    
        public virtual ICollection<AspNetDeployExceptionEntry> AspNetDeployException { get; set; }
        public virtual ICollection<ExceptionEntry> ParentExceptionEntry { get; set; }
        public virtual ExceptionEntry InnerExceptionEntry { get; set; }
        public virtual ICollection<ExceptionEntryData> ExceptionData { get; set; }
        public virtual ICollection<MachinePublicationLog> MachinePublicationLog { get; set; }
    }
}
