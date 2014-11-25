using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models
{
    public class VariableAddModel
    {
        [Required]
        public int EnvironmentId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        public string Value { get; set; }
    }
}