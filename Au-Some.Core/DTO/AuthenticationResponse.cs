using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Au_Some.Core.DTO
{
    public class AuthenticationResponse
    {
        public string ChildName {  get; set; }=string.Empty;
        public string Email { get; set; }=string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpirationDateTime { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpirationDateTime { get; set; }

    }
}
