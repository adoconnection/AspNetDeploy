using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetDeploy.WebUI.Models
{
    public class CreateNewGitVersion: CreateNewSourceControlVersion
    {
        [Required]
        public string NewVersionBranch { get; set; }
    }
}