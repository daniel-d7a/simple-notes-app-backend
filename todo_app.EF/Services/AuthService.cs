using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using todo_app.core.Constants;
using todo_app.core.Models.Auth;
using todo_app.core.Models.ResponseModels.Auth;
using todo_app.core.Services;

namespace todo_app.EF.Services
{
    public class AuthService(UserManager<UserModel> userManager, IConfiguration configuration)
        : IAuthService
    {
        private async Task<JwtSecurityToken> CreateToken(UserModel user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim("role", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var signingCredintials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                signingCredentials: signingCredintials,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    Convert.ToDouble(configuration["JWT:DurationInDays"])
                )
            );
            return token;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthResponse { Message = "This Email Already Exists" };
            }

            if (await userManager.FindByNameAsync(model.Username) is not null)
            {
                return new AuthResponse { Message = "This Username Already Exists" };
            }
            var user = new UserModel { UserName = model.Username, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Message = "User was not created",
                    Errors = result.Errors.Select(e => e.Description).ToList(),
                };
            }

            await userManager.AddToRoleAsync(user, RoleNames.USER);

            var token = await CreateToken(user);

            return new AuthResponse
            {
                User = new()
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Id = user.Id
                },
                Message = "User Registered Successfully",
                Roles = [RoleNames.USER],
                IsAuthenticated = true,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    Convert.ToDouble(configuration["JWT:DurationInDays"])
                ),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResponse { Message = "Email Or Password is Incorrect" };
            }

            var token = await CreateToken(user);
            var roles = await userManager.GetRolesAsync(user);

            return new AuthResponse
            {
                User = new()
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Id = user.Id
                },
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                IsAuthenticated = true,
                Message = "User logged in successfully",
                ExpiresAt = DateTime.UtcNow.AddDays(
                    Convert.ToDouble(configuration["JWT:DurationInDays"])
                ),
                Roles = roles.ToList()
            };
        }
    }
}
