using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public abstract class CreateNewSourceControlVersion
    {
        [Required]
        public int FromSourceControlVersionId { get; set; }

        [Required]
        public string NewVersionName { get; set; }
    }
}