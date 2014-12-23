using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class CreateNewFileSystemVersion : CreateNewSourceControlVersion
    {
        [Required]
        public string NewVersionPath { get; set; }  
    }
}