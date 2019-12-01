using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(15, ErrorMessage = "Maximum length of 15 characters")]
        public string NickName { get; set; }

        public string ReturnUrl { get; set; }
        public string LoginProvider { get; set; }
    }
}
