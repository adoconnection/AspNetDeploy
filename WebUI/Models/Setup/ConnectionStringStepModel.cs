using System.ComponentModel.DataAnnotations;

namespace AspNetDeploy.WebUI.Models.Setup
{
    public class ConnectionStringStepModel
    {
        [Required]
        public string ConnectionString { get; set; }
    }
}