using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetDeploy.WebUI.Models.SourceControls
{
    public class AddGitModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}