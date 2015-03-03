using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.Model
{
    public enum UserRole
    {
        [Display(Name = "Undefined")]
        Undefined = 0,

        [Display(Name = "Observer")]
        Observer = 1,

        [Display(Name = "Developer")]
        Developer = 10,

        [Display(Name = "Power Developer")]
        PowerDeveloper = 20,

        [Display(Name = "Tester")]
        Tester = 100,

        [Display(Name = "Publisher")]
        Publisher = 1000,

        [Display(Name = "Admin")]
        Admin = 10000
    }
}