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
    
    public partial class DataFieldValue
    {
        public DataFieldValue()
        {
            this.Environments = new HashSet<Environment>();
            this.Machines = new HashSet<Machine>();
        }
    
        public int Id { get; set; }
        public int DataFieldId { get; set; }
        public string Value { get; set; }
    
        public virtual DataField DataField { get; set; }
        public virtual ICollection<Environment> Environments { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
    }
}
