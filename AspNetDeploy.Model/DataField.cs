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
    
    public partial class DataField
    {
        public DataField()
        {
            this.DataFieldValues = new HashSet<DataFieldValue>();
            this.BundleVersions = new HashSet<BundleVersion>();
        }
    
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Key { get; set; }
        public bool IsSensitive { get; set; }
        public int ModeId { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ICollection<DataFieldValue> DataFieldValues { get; set; }
        public virtual ICollection<BundleVersion> BundleVersions { get; set; }
    }
}
