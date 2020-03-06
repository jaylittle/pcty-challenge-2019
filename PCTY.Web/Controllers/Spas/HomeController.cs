using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using PCTY.Shared;

namespace PCTY.Web.Controllers.Spas
{
  [Route("")]
  [ResponseCache(CacheProfileName = "None")]
  [Authorize(Roles = "PCTYAdmin")]
  public class HomeController : Controller
  {
    IServiceProvider _svp; 
    IHttpContextAccessor _httpAccessor;
    public HomeController(IServiceProvider svp, IHttpContextAccessor httpAccessor)
    {
      _svp = svp;
      _httpAccessor = httpAccessor;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
      var state = new Models.AppStateModel(_svp, Settings.Current, _httpAccessor.HttpContext, "Home", "home");
      return View("Spa", state);
    }

    [HttpGet("home")]
    public IActionResult Home()
    {
      return Redirect($"{Settings.Current.RelativeBaseUrl}");
    }

    [HttpGet("/home/{*path}")]
    public IActionResult SubHome(string path)
    {
      return Index();
    }
  }
}