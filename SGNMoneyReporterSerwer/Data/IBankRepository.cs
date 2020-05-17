using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SGNMoneyReporterSerwer.Data.Entities;
using SGNMoneyReporterSerwer.Models;

namespace SGNMoneyReporterSerwer.Data
{
    public interface IBankRepository
    {
        //void Add<T>(T entity) where T : class;
        //void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        //Configurations & Dictionaries
        Task<List<Quality>> GetAllQualitiesAsync();
        Task<List<Mode>> GetAllModesAsync();
        Task<List<Machine>> GetAllMachinesAsync();

        Task<List<Currency>> GetAllCurrenciesAsync();
        Task<List<CurrencyFaceValue>> GetAllFacesValuesAsync();
        Task<List<Role>> GetRoles();


        Task<List<QualitySP>> GetFilteredValuesAsync(string quality, string currency, string mode, DateTime begin, DateTime end);
        Task<List<QualityWithMachineSP>> GetFilteredValuesExtAsync(string quality, string currency, string mode, DateTime begin, DateTime end);

        Task<List<SerialNumbersDuplicatesSP>> GetAllDuplicatesFiltered(string currency, DateTime begin, DateTime end);
        Task<List<FileHistory>> GetAllFilesHistoryAsync();

    }
}
