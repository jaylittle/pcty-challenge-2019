using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using PCTY.Shared.Models;
using PCTY.Data.Interfaces;
using PCTY.Logic.Interfaces;
using PCTY.Logic;

namespace PCTY.Web.Controllers.Spas
{
  [Security.XSRF.XSRFCheck]
  [Route("/api/[controller]")]
  [ResponseCache(CacheProfileName = "None")]
  [Authorize(Roles = "PCTYAdmin")]
  public class EmployeeController : Controller
  {
    IServiceProvider _svp; 
    IHttpContextAccessor _httpAccessor;
    IEmployeeDal _employeeDal;
    IEmployeeBL _employeeBl;

    public EmployeeController(IServiceProvider svp, IHttpContextAccessor httpAccessor, IEmployeeDal employeeDal, IEmployeeBL employeeBl)
    {
      _svp = svp;
      _httpAccessor = httpAccessor;
      _employeeDal = employeeDal;
      _employeeBl = employeeBl;
    }

    [HttpGet()]
    public async Task<IActionResult> ListEmployees()
    {
      return this.Ok(await _employeeBl.ListEmployees());
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetEmployee(Guid? guid = null)
    {
      if (guid.HasValue && !Guid.Empty.Equals(guid))
      {
        return this.Ok(await _employeeBl.GetEmployee(guid.Value));
      }
      return this.BadRequest();
    }

    [HttpPost()]
    [HttpPut()]
    public async Task<IActionResult> UpsertEmployee([FromBody]EmployeeModel record = null)
    {
      if (record != null)
      {
        var result = await _employeeBl.UpsertEmployee(record);
        if (result.Successful)
        {
          return this.Ok(record);
        }
        return this.BadRequest(result.LogMessages.Select(lm => lm.Text));
      }
      return this.BadRequest();
    }

    [HttpPost("action/calculate-benefit-cost")]
    public IActionResult GetBenefitCosts([FromBody]EmployeeModel record = null)
    {
      if (record != null)
      {

        return this.Ok(record.GetBenefitCosts());
      }
      return this.BadRequest();
    }
  }
}