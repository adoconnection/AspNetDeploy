using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.Setup
{
    public class UsersStepModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }
}