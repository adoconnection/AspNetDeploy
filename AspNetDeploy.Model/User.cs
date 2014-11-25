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
    
    public partial class User
    {
        public User()
        {
            this.PackageApprovedOnEnvironment = new HashSet<PackageApprovedOnEnvironment>();
            this.Publication = new HashSet<Publication>();
        }
    
        public int Id { get; set; }
        public System.Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public bool IsDisabled { get; set; }
    
        public virtual ICollection<PackageApprovedOnEnvironment> PackageApprovedOnEnvironment { get; set; }
        public virtual ICollection<Publication> Publication { get; set; }
    }
}
