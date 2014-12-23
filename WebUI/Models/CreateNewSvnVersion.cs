using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class CreateNewSvnVersion : CreateNewSourceControlVersion
    {
        [Required]
        public string NewVersionURL { get; set; } 
    }
}