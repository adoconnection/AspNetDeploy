using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.SourceControls
{
    public class AddSvnModel
    {
        [Required]
         public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}