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
                IdCurrencyFaceValue = (short)reader["IdCurrencyFaceValue"],
                FaceValue = (decimal)reader["FaceValue"],
                //    CountedCount = (int)reader["CountedCount"],
                Count = (int)reader["Counts"],
                QualityValue = (string)reader["QualityValue"],
                Symbol = (string)reader["Symbol"],
                ModeValue = (string)reader["ModeValue"],
                IdCurrency = (short)reader["IdCurrency"]
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
                IdCurrencyFaceValue = (short)reader["IdCurrencyFaceValue"],
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

        public async Task<List<SerialNumbersDuplicatesSP>> GetAllDuplicatesFiltered(string currency,  DateTime begin, DateTime end)
        {
            using (SqlConnection sql = new SqlConnection(_connection))
            {
                SqlCommand cmd = new SqlCommand("SerialNumberDuplicates", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                //cmd.Parameters.Add(new SqlParameter("@idMode", idMode));
                //cmd.Parameters.Add(new SqlParameter("@idQuality", idQual));
                cmd.Parameters.Add(new SqlParameter("@idCurrency ", currency));
                cmd.Parameters.Add(new SqlParameter("@startDate ", begin));
                cmd.Parameters.Add(new SqlParameter("@endDate ", end));
                var response = new List<SerialNumbersDuplicatesSP>();
                await sql.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        response.Add(MapToValueDuplicate(reader));
                    }
                }
                cmd.CommandTimeout = 30;
                return response;
            }
        }

        private SerialNumbersDuplicatesSP MapToValueDuplicate(SqlDataReader reader)
        {
            return new SerialNumbersDuplicatesSP()
            {
                //IdMachine = (int)reader["IdMachine"],
                SN = (string)reader["SN"],
                Counts = (int)reader["Counts"],
                BanknoteSN = (string)reader["BanknoteSN"],
                IdCurrencyFaceValue = (short)reader["IdCurrencyFaceValue"],
                IdCurrency = (short)reader["IdCurrency"],
                Symbol = (string)reader["Symbol"],
                FaceValue = (decimal)reader["FaceValue"]
            };
        }

        public async Task<List<FileHistory>> GetAllFilesHistoryAsync()
        {
            IQueryable<FileHistory> query = _context.FileHistory.Where(r => !r.IsProceededSuccess);
                //.Take(30);
            query = query.OrderBy(x => x.IdFileHistory);
            return await query.ToListAsync();
        }

        public async Task<List<Role>> GetRoles()
        {
            IQueryable<Role> query = _context.Roles.Where(r=>r.RoleId>1);
            query = query.OrderBy(x => x.RoleId);
            return await query.ToListAsync();
        }
    }
}
