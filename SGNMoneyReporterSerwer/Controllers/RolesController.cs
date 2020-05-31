using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGNMoneyReporterSerwer.Data;
using SGNMoneyReporterSerwer.Models;

namespace SGNMoneyReporterSerwer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IBankRepository _repository;
        private readonly IMapper _mapper;
        public RolesController(IBankRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository)); ;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleModel>>> Get()
        {
            try
            {
                var result = await _repository.GetRoles();
                return _mapper.Map<List<RoleModel>>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Błąd połączenia z bazą danych");
            }
        }
    }
}