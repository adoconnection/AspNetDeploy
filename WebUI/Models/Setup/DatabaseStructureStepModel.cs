using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.Setup
{
    public class DatabaseStructureStepModel
    {
        [Required]
        public string Keyword { get; set; } 
        public bool IsReady { get; set; } 
    }
}