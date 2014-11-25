using System;
using System.ComponentModel.DataAnnotations;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class UserBaseModel
    {
        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Заблокировать")]
        public bool IsDisabled { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public UserRole Role { get; set; }
    }
}