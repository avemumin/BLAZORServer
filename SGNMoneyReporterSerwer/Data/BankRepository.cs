using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SGNMoneyReporterSerwer.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGNMoneyReporterSerwer.Models;

namespace SGNMoneyReporterSerwer.Data
{
    
    public class BankRepository : IBankRepository
    {
        private readonly BankContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connection;
        private readonly JWTSettings _jwtSettings;
        public BankRepository(BankContext context, IConfiguration configuration, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _configuration = configuration;
            _connection = configuration.GetConnectionString("ConnectionDB");
            _jwtSettings = jwtSettings.Value;
        }
        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Dictionary of Qualities
        /// </summary>
        /// <returns></returns>
        public async Task<List<Quality>> GetAllQualitiesAsync()
        {
            IQueryable<Quality> query = _context.Quality;
            query = query.OrderBy(x => x.IdQuality);
            return await query.ToListAsync();

        }
        /// <summary>
        /// Dictionary of Mode
        /// </summary>
        /// <returns></returns>
        public async Task<List<Mode>> GetAllModesAsync()
        {
            IQueryable<Mode> query = _context.Mode;
            query = query.OrderBy(x => x.IdMode);
            return await query.ToListAsync();

        }
        /// <summary>
        /// Dictionary of Machine
        /// </summary>
        /// <returns></returns>
        public async Task<List<Machine>> GetAllMachinesAsync()
        {
            IQueryable<Machine> query = _context.Machine;
            query = query.OrderBy(x => x.IdMachine);
            return await query.ToListAsync();
        }
        /// <summary>
        /// Dictionary of Currency
        /// </summary>
        /// <returns></returns>
        public async Task<List<Currency>> GetAllCurrenciesAsync()
        {
            IQueryable<Currency> query = _context.Currency;
            query = query.OrderBy(x => x.IdCurrency);
            return await query.ToListAsync();
        }
        /// <summary>
        /// Dictionary of Faces
        /// </summary>
        /// <returns></returns>
        public async Task<List<CurrencyFaceValue>> GetAllFacesValuesAsync()
        {
            IQueryable<CurrencyFaceValue> query = _context.CurrencyFaceValue;
            query = query.OrderBy(x => x.IdCurrencyFaceValue);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Quality report
        /// </summary>
        /// <param name="quality"></param>
        /// <param name="currency"></param>
        /// <param name="mode"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<List<QualitySP>> GetFilteredValuesAsync(string quality, string currency, string mode, DateTime begin, DateTime end)
        {
            await using (SqlConnection sql = new SqlConnection(_connection))
            {
                SqlCommand cmd = new SqlCommand("CurrencyQualityRepoSp", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idMode", mode));
                cmd.Parameters.Add(new SqlParameter("@idQuality", quality));
                cmd.Parameters.Add(new SqlParameter("@idCurrency ", currency));
                cmd.Parameters.Add(new SqlParameter("@startDate ", begin));
                cmd.Parameters.Add(new SqlParameter("@endDate ", end));
                var response = new List<QualitySP>();
                await sql.OpenAsync();
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        response.Add(MapToValue(reader));
                    }
                }

                return response;
            }
        }
        /// <summary>
        /// Mapper between execution sql procedure (CurrencyQualityRepoSp) and property in model
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private QualitySP MapToValue(SqlDataReader reader)
        {
            return new QualitySP()
            {
                // IdCurrencyFaceValue = (short)reader["IdCurrencyFaceValue"],
                FaceValue = (decimal)reader["FaceValue"],
                //    CountedCount = (int)reader["CountedCount"],
                Count = (int)reader["Counts"],
                QualityValue = (string)reader["QualityValue"],
                Symbol = (string)reader["Symbol"],
                ModeValue = (string)reader["ModeValue"]
            };
        }
        /// <summary>
        /// Extender stored procedure with machines
        /// </summary>
        /// <param name="quality"></param>
        /// <param name="currency"></param>
        /// <param name="mode"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<List<QualityWithMachineSP>> GetFilteredValuesExtAsync(string quality, string currency, string mode, DateTime begin, DateTime end)
        {
            await using (SqlConnection sql = new SqlConnection(_connection))
            {
                SqlCommand cmd = new SqlCommand("CurrencyQualityRepoMachineSp", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idMode", mode));
                cmd.Parameters.Add(new SqlParameter("@idQuality", quality));
                cmd.Parameters.Add(new SqlParameter("@idCurrency ", currency));
                cmd.Parameters.Add(new SqlParameter("@startDate ", begin));
                cmd.Parameters.Add(new SqlParameter("@endDate ", end));
                var response = new List<QualityWithMachineSP>();
                await sql.OpenAsync();
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        response.Add(MapToValueMachine(reader));
                    }
                }

                return response;
            }
        }

        private QualityWithMachineSP MapToValueMachine(SqlDataReader reader)
        {
            return new QualityWithMachineSP()
            {
                //   IdCurrencyFaceValue = (short)reader["IdCurrencyFaceValue"],
                IdMachine = (int)reader["IdMachine"],
                SN = (string)reader["SN"],
                FaceValue = (decimal)reader["FaceValue"],
                //     CountedCount = (int)reader["CountedCount"],
                Count = (int)reader["Counts"],
                QualityValue = (string)reader["QualityValue"],
                Symbol = (string)reader["Symbol"],
                ModeValue = (string)reader["ModeValue"]
            };
        }





        //public async Task<List<User>> GetUsers()
        //{
        //    var query = await _context.User.ToListAsync();
        //    return query;

        //}

        //public async Task<User> GetUser(int id)
        //{
        //    var query = await _context.User.FindAsync(id);
        //    return query;
        //}

        //public async Task<User> GetUser(string email)
        //{
        //    var query = await _context.User
        //        .Where(user => user.UserEmailAddress == email).FirstOrDefaultAsync();

        //    return query;
        //}

        //public async Task<UserWithToken> Login([FromBody] User user)
        //{
        //    user = await _context.User
        //        .Where(x => x.UserEmailAddress == user.UserEmailAddress && x.UserPassword == user.UserPassword)
        //        .FirstOrDefaultAsync();

        //    UserWithToken userWithToken = null; // new UserWithToken(query);
        //    if (user != null)
        //    {
        //        RefreshToken refreshToken = GenerateRefreshToken();
        //        user.RefreshTokens.Add(refreshToken);
        //        await _context.SaveChangesAsync();

        //        userWithToken = new UserWithToken(user);
        //        userWithToken.RefreshToken = refreshToken.Token;
        //    }

        //    if (userWithToken == null)
        //    {
        //        return null;
        //    }
        //    userWithToken.AccessToken = GenerateAccessToken(user.IdUser);

        //    return userWithToken;
        //}
        //[HttpPost("RefreshToken")]
        //public async Task<UserWithToken> RefreshToken([FromBody] RefreshRequest refreshRequest)
        //{
        //    UserModel user = GetUserFromAccessToken(refreshRequest.AccessToken);
        //    if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
        //    {

        //    }
        //    return null;
        //}

        //private bool ValidateRefreshToken(UserModel user, string refreshToken)
        //{
        //    throw new NotImplementedException();
        //}

        //private async Task<User> GetUserFromAccessToken(string accessToken)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        //        var TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ClockSkew = TimeSpan.Zero
        //        };
        //        SecurityToken securityToken;
        //        var principal = tokenHandler.ValidateToken(accessToken, TokenValidationParameters, out securityToken);
        //        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        //        if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
        //            StringComparison.CurrentCultureIgnoreCase))
        //        {
        //            var uid = principal.FindFirst(ClaimTypes.Name)?.Value;
        //            return await _context.User.Where(x => x.IdUser == Convert.ToInt32(uid)).FirstOrDefaultAsync();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return new User();
        //    }

        //    return new User();
        //}

        ////public async Task<UserWithToken> Login([FromBody] UserWithToken user)
        ////{
        ////    var query = await _context.User
        ////        .Where(x => x.UserEmailAddress == user.UserEmailAddress && x.UserPassword == user.UserPassword)
        ////        .FirstOrDefaultAsync();

        ////    UserWithToken userWithToken = null;// new UserWithToken(query);

        ////    //var tokenHandler = new JwtSecurityTokenHandler();
        ////    //var key = Encoding.ASCII.GetBytes(_jwtSettings.SK);

        ////    //var tokenDescriptor = new SecurityTokenDescriptor
        ////    //{
        ////    //    Subject = new ClaimsIdentity(new Claim[]
        ////    //    {
        ////    //        new Claim(ClaimTypes.Name, user.UserEmailAddress)
        ////    //    }),
        ////    //    Expires = DateTime.Now.AddMinutes(15),
        ////    //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        ////    //        SecurityAlgorithms.HmacSha256Signature)
        ////    //};

        ////    //var token = tokenHandler.CreateToken(tokenDescriptor);
        ////    //tokenHandler.WriteToken(token);
        ////    //userWithToken.Token
        ////    if (user != null)
        ////    {
        ////        RefreshToken refreshToken = GenerateRefreshToken();
        ////        user.
        ////    }

        ////    return userWithToken;
        ////}

        //private RefreshToken GenerateRefreshToken()
        //{
        //    RefreshToken refreshToken = new RefreshToken();

        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        refreshToken.Token = Convert.ToBase64String(randomNumber);
        //    }
        //    refreshToken.ExpiryDate = DateTime.Now.AddHours(5);

        //    return refreshToken;
        //}
        //private string GenerateAccessToken(int userId)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, Convert.ToString(userId))
        //        }),
        //        Expires = DateTime.Now.AddHours(2),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        //            SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
