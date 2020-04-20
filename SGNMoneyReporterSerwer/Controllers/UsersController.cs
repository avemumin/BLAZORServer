using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGNMoneyReporterSerwer.Data.Entities;
using SGNMoneyReporterSerwer.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SGNMoneyReporterSerwer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly JWTSettings _jwtSettings;
        public UsersController(BankContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpGet("GetUserDetails/{id}")]
        public async Task<ActionResult<User>> GetUserDetails(int id)
        {
            var user = await _context.User
                .Where(u => u.IdUser == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserWithToken>> Login([FromBody] User user)
        {
            user = await _context.User
                .Where(u => u.UserEmailAddress == user.UserEmailAddress
                            && u.UserPassword == user.UserPassword).FirstOrDefaultAsync();

            UserWithToken userWithToken = null;

            if (user != null)
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                userWithToken = new UserWithToken(user);
                userWithToken.RefreshToken = refreshToken.Token;
            }

            if (userWithToken == null)
            {
                return NotFound();
            }


            userWithToken.AccessToken = GenerateAccessToken(user.IdUser);
            return userWithToken;
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            User user = await GetUserFromAccessToken(refreshRequest.AccessToken);

            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                UserWithToken userWithToken = new UserWithToken(user);
                userWithToken.AccessToken = GenerateAccessToken(user.IdUser);

                return userWithToken;
            }

            return null;
        }

        [HttpPost("GetUserByAccessToken")]
        public async Task<ActionResult<User>> GetUserByAccessToken([FromBody] string accessToken)
        {
            User user = await GetUserFromAccessToken(accessToken);

            if (user != null)
            {
                return user;
            }

            return null;
        }


        private bool ValidateRefreshToken(User user, string refreshToken)
        {

            RefreshToken refreshTokenUser = _context.RefreshToken.Where(rt => rt.Token == refreshToken)
                .OrderByDescending(rt => rt.ExpiryDate)
                .FirstOrDefault();

            if (refreshTokenUser != null && refreshTokenUser.UserId == user.IdUser
                                         && refreshTokenUser.ExpiryDate > DateTime.Now)
            {
                return true;
            }

            return false;
        }


        private async Task<User> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.Name)?.Value;

                    return await _context.User
                        .Where(u => u.IdUser == Convert.ToInt32(userId)).FirstOrDefaultAsync();
                }
            }
            catch (Exception)
            {
                return new User();
            }

            return new User();
        }
        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.Now.AddHours(1);

            return refreshToken;
        }
        private string GenerateAccessToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(userId))
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}