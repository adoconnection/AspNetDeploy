using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class UserEditModel : UserBaseModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Должен быть не менее 6 символов", MinimumLength = 6)]
        public string Password { get; set; }
    }
}