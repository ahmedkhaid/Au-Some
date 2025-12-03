using Au_Some.Core.DTO;
using Au_Some.Core.Identity;
using Au_Some.Core.ServiceContract;
using Au_Some.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Au_Some.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        public AccountController(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole>roleManager,SignInManager<ApplicationUser>signInManager,IJwtService jwtService)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _signInManager=signInManager;
            _jwtService=jwtService;
        }
        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> Register(RegisterDto registerUser) {
            if(ModelState.IsValid==false)
            {
                string error=string.Join("\n",ModelState.Values.SelectMany(x => x.Errors).SelectMany(e=>e.ErrorMessage));
                return Problem(error);
            }
            ApplicationUser user = new()
            {
                Email = registerUser.Email,
                UserName=registerUser.Email,
                ChildName=registerUser.ChildName,
                Age=registerUser.Age,
            };
           var registered= await _userManager.CreateAsync(user,registerUser.Password);
            if(registered.Succeeded==false)
            {
                string errors = string.Join("|", registered.Errors.Select(e => e.Description));
                return Problem(errors);
            }
           await _signInManager.SignInAsync(user, isPersistent: true);
            AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);

            user.RefreshToken=authenticationResponse.RefreshToken;

            user.RefreshTokenExpirationDateTime=authenticationResponse.RefreshTokenExpirationDateTime;
            await _userManager.UpdateAsync(user);
            return Ok(authenticationResponse);
        }
        [HttpPost]
        public async Task<IActionResult>Login(LoginDto loginUser)
        {
            if (ModelState.IsValid==false)
            {
                string error = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors).SelectMany(e => e.ErrorMessage));
                return Problem(error);
            }
            var result= await _signInManager.PasswordSignInAsync(loginUser.Email,loginUser.Password,isPersistent:true,lockoutOnFailure:false);
            if (result.Succeeded==true) {
                ApplicationUser ?user = await _userManager.FindByEmailAsync(loginUser.Email);
                if(user==null)
                {
                    return NoContent();
                }
                AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);
                user.RefreshToken=authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime=authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);
                return Ok(authenticationResponse);
                
            }
            return Problem("Email or password invalid");
        }
        [HttpPost]
        public async Task<IActionResult> GenerateRefersehToken(TokenModel tokenModel)
        {
            if(tokenModel==null)
            {
                return BadRequest("InValid Client Request");
            }
            ClaimsPrincipal ?claims =  _jwtService.GetClaimsPrincipal(tokenModel.AccessToken);
            if(claims==null)
            {
                return BadRequest("Invalid Jwt Token");
            }
            string? email = claims.FindFirstValue(ClaimTypes.Email);
            ApplicationUser ?user =await _userManager.FindByEmailAsync(email);
            if(user ==null || user.RefreshToken!=tokenModel.RefreshToken || user.RefreshTokenExpirationDateTime<=DateTime.Now)
            {
                return BadRequest("Invalid Refresh Token");
            }
            AuthenticationResponse authenticationResponse = _jwtService.CreateJwtToken(user);
            user.RefreshToken=authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
              await _userManager.UpdateAsync(user);
            return Ok(authenticationResponse);
        }
    }
}
