using System.ComponentModel.DataAnnotations;

namespace IdentityServerX.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
