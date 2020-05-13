using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGNMoneyReporterSerwer.Data;
using SGNMoneyReporterSerwer.Data.Entities;

namespace SGNMoneyReporterSerwer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerialNumbersDuplicatesController : ControllerBase
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;
        public SerialNumbersDuplicatesController(IBankRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{currency}/{start}/{end}")]
        public async Task<ActionResult<List<SerialNumbersDuplicatesSP>>> Get(string currency, DateTime start, DateTime end)
        {
            try
            {
                var result = await _repository.GetAllDuplicatesFiltered(currency, start, end);
                if (result == null) return NotFound();
                return _mapper.Map<List<SerialNumbersDuplicatesSP>>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Błąd połączenia z bazą danych");
            }
        }
    }
}