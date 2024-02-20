using System;
using System.ComponentModel.DataAnnotations;
using AspNetDeploy.Model;

namespace AspNetDeploy.WebUI.Models
{
    public class UserBaseModel
    {
        [Required]
        [Display(Name = "Display name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Is disabled")]
        public bool IsDisabled { get; set; }

        [Required]
        [Display(Name = "Role")]
        public UserRole Role { get; set; }

        [Required]
        [Display(Name = "Theme")]
        public string ThemeId { get; set; }
    }
}