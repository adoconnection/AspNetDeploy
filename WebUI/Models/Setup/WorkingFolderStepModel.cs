using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.Setup
{
    public class WorkingFolderStepModel
    {
        [Required]
        public string Path { get; set; } 
    }
}