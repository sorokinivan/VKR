using System.ComponentModel.DataAnnotations;

namespace VKR.Models.AuthorizationModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
