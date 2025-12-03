using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Au_Some.Core.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="The Child name can't be Empty")]
        public string ChildName { get; set; }=string.Empty;
        [EmailAddress]
        [Required(ErrorMessage ="The Email can't be Empty")]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already is use")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage ="the Password can't be Empty")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage ="The Confirm Password can't be Empty")]
        [Compare(nameof(Password),ErrorMessage ="The Confirmed Password should be equalt to Password")]
        public string ConfirmedPassword { get; set; } = string.Empty;
        [Required(ErrorMessage ="The Age cant be Empty")]
        public int Age { get; set; }
    }
}
