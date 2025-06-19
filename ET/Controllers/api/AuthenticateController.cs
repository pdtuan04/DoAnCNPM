using ET.Models;
using Libs.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        public AuthenticateController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Registeration(RegistrationModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return Ok(new { status = false, message = "Tên đăng nhập dã tồn tại" });

            User user = new();
            user.UserName = model.Username;
            user.Email = model.Email;

            var createUserResult = await userManager.CreateAsync(user, model.Password);


            return Ok(new { status = true, message = "Đăng Ký Tài Khoản Thành Công" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Ok(new { status = true, message = "Invalid username" });
            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return Ok(new { status = true, message = "Invalid password" });

            var userRoles = await userManager.GetRolesAsync(user);


            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var roleIdentity = roleManager.Roles.Where(s => userRoles.Contains(s.Name)).ToList();
            //var claims = roleManager.GetClaimsAsync(roleIdentity).Result;
            foreach (var userRole in roleIdentity)
            {
                var claims = roleManager.GetClaimsAsync(userRole).Result;
                for (int i = 0; i < claims.Count; i++)
                {
                    authClaims.Add(claims[i]);
                }
                authClaims.Add(new Claim(ClaimTypes.Role, userRole.Name));
            }


            string token = GenerateToken(authClaims);
            return Ok(new { status = true, message = "", token = token });
        }
        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [HttpPost("forgot-password")]
        private async Task<IActionResult> forgotPassWord()
        {

            return Ok(new { status = true, message = "Chức năng này chưa được triển khai" });
        }
    }
}
