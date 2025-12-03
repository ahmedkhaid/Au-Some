using Au_Some.Core.DTO;
using Au_Some.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Au_Some.Core.ServiceContract
{
    public interface IJwtService
    {
        public AuthenticationResponse CreateJwtToken(ApplicationUser user);
        public string RefreshToken();
        public ClaimsPrincipal? GetClaimsPrincipal(string?token);
    }
}
