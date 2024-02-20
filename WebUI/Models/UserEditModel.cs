using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class UserEditModel : UserBaseModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Must be at least 6 ", MinimumLength = 6)]
        public string Password { get; set; }
    }
}