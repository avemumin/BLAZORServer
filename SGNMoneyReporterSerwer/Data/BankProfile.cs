using AutoMapper;
using SGNMoneyReporterSerwer.Data.Entities;
using SGNMoneyReporterSerwer.Models;

namespace SGNMoneyReporterSerwer.Data
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            this.CreateMap<Quality, QualityModel>();
            this.CreateMap<Machine, MachineModel>();
            this.CreateMap<Mode, ModeModel>();
            this.CreateMap<Currency, CurrencyModel>();
            this.CreateMap<CurrencyFaceValue, CurrencyFaceValueModel>();
            this.CreateMap<QualitySP, QualitySPModel>();
            this.CreateMap<QualityWithMachineSP, QualitySPMachineModel>();

        }
 
    }
}
