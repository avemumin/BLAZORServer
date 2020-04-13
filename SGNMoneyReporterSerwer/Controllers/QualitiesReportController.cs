using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SGNMoneyReporterSerwer.Data;
using SGNMoneyReporterSerwer.Models;

namespace SGNMoneyReporterSerwer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QualitiesReportController : ControllerBase
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;

        public QualitiesReportController(IBankRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper;
        }

        [HttpGet("{quality}/{currency}/{mode}/{start}/{end}")]
        public async Task<ActionResult<List<QualitySPModel>>> Get(string quality, string currency, string mode, DateTime start, DateTime end)
        {
            try
            {
                var result = await _repository.GetFilteredValuesAsync(quality, currency, mode, start, end);
                if (result == null) return NotFound();
                return _mapper.Map<List<QualitySPModel>>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Błąd połączenia z bazą danych");
            }
        }
    }
}