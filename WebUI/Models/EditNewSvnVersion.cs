using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class EditNewSvnVersion
    {
        [Required]
        public int Id { get; set; } 
    }
}