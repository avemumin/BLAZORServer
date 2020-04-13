using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SGNMoneyReporterSerwer.Data.Entities;
using Microsoft.Extensions.Configuration;
namespace SGNMoneyReporterSerwer.Data
{
    public class BankRepository : IBankRepository
    {
        private readonly BankContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _connection;
        public BankRepository(BankContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connection = configuration.GetConnectionString("ConnectionDB");
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
    }
}
