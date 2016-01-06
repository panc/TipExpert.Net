using System.ComponentModel.DataAnnotations;

namespace TipExpert.Net.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
