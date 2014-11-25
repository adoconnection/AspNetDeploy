using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class VariableEditModel : VariableAddModel
    {
        [Required]
        public int VariableId { get; set; }
    }
}