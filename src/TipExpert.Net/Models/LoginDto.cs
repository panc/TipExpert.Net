using System.ComponentModel.DataAnnotations;

namespace TipExpert.Net.Models
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Remember me?")]
        public bool rememberMe { get; set; }
    }
}