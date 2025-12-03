using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Au_Some.Core.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "the email can't be Empty")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "the Password can't be Empty")]
        public string Password { get; set; } = string.Empty;
    }
}
