using System.ComponentModel.DataAnnotations;

namespace Project_PRN222.DTO
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; } // Optional

        public string Address { get; set; } // Optional
    }
}
