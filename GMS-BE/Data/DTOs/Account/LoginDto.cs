using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GMS_BE.Data.DTOs.Account
{
    public class LoginDto
    {
        [Required]
        public string  UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}