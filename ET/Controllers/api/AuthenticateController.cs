using ET.Models;
using Libs.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using ET.Services;
using Hangfire;

namespace ETAuthApp.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private IGmailSender gmailSender;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public AuthenticateController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IBackgroundJobClient backgroundJobClient, IGmailSender gmailSender)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            this.gmailSender = gmailSender;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registeration([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = false, message = "Dữ liệu không hợp lệ" });

            var userNameExists = await userManager.FindByNameAsync(model.Username);
            if (userNameExists != null)
                return Ok(new { status = false, message = "Tên đăng nhập đã tồn tại" });
            var userEmailExists = await userManager.FindByEmailAsync(model.Email);
            if (userEmailExists != null)
                return Ok(new { status = false, message = "Email đã tồn tại" });
            User user = new()
            {
                UserName = model.Username,
                Email = model.Email
            };

            var createUserResult = await userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description);
                return Ok(new { status = false, message = "Đăng ký thất bại", errors });
            }

            string role = "User";
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                    return Ok(new { status = false, message = "Tạo vai trò thất bại", errors = roleResult.Errors.Select(e => e.Description) });
            }
            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
                return Ok(new { status = false, message = "Gán vai trò thất bại", errors = addRoleResult.Errors.Select(e => e.Description) });
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Welcome.html");
            var template = await System.IO.File.ReadAllTextAsync(templatePath);
            string body = template
                .Replace("{user.UserName}", user.UserName)
                .Replace("{user.FullName}", user.UserName);
            _backgroundJobClient.Enqueue(() => gmailSender.SendEmailAsync(user.Email, "Chào mừng đến với ProTest!", body));
            return Ok(new { status = true, message = "Đăng ký tài khoản thành công", role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = false, message = "Dữ liệu không hợp lệ" });

            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Ok(new { status = false, message = "Tên đăng nhập không tồn tại" });

            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return Ok(new { status = false, message = "Mật khẩu không đúng" });

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Any())
                return Ok(new { status = false, message = "Tài khoản không có vai trò" });

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = GenerateToken(authClaims);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Tạm tắt Secure khi debug local
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JWTKey:TokenExpiryTimeInHour"]))
            });

            return Ok(new { status = true, message = "Đăng nhập thành công", token, roles = userRoles });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { status = true, message = "Đăng xuất thành công" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            return Ok(new { status = false, message = "Chức năng này chưa được triển khai" });
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JWTKey:TokenExpiryTimeInHour"])),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}