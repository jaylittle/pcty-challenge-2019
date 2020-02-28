using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using PCTY.Shared;
using PCTY.Web.Security;

namespace PCTY.Web.Controllers.Spas
{
  [Route("log")]
  [ResponseCache(CacheProfileName = "None")]
  public class LoginController : Controller
  {
    IServiceProvider _svp; 
    IHttpContextAccessor _httpAccessor;
    public LoginController(IServiceProvider svp, IHttpContextAccessor httpAccessor)
    {
      _svp = svp;
      _httpAccessor = httpAccessor;
    }

    [HttpGet("in")]
    public IActionResult In(bool? authFailed = false, bool? azure = false, string successUrl = null, bool? expired = null, bool? manual = null)
    {
      Middleware.TokenCookieMiddleware.RemoveJwtCookie(_httpAccessor.HttpContext);
      Middleware.TokenCookieMiddleware.RemoveXsrfCookie(_httpAccessor.HttpContext);
      var state = new Models.AppStateModel(_svp, Settings.Current, _httpAccessor.HttpContext, "Login", "login");

      return View("Spa", state);
    }

    [HttpGet("out")]
    public IActionResult Out(bool? expired)
    {
      Middleware.TokenCookieMiddleware.RemoveJwtCookie(_httpAccessor.HttpContext);
      Middleware.TokenCookieMiddleware.RemoveXsrfCookie(_httpAccessor.HttpContext);
      
      //Blacklist current auth token (if applicable)
      if (_httpAccessor.HttpContext.Request?.HttpContext?.User != null && _httpAccessor.HttpContext.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        string token = _httpAccessor.HttpContext.Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization"))
          .Value.FirstOrDefault()?.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrWhiteSpace(token))
        {
          Middleware.TokenReplayCache.Instance.Expire(token);
        }
      }

      string queryString = string.Empty;
      if (expired.HasValue && expired.Value)
      {
        queryString = "?expired=true";
      }
      else
      {
        queryString = "?manual=true";
      }
      return Redirect($"{Settings.Current.RelativeBaseUrl}log/in" + queryString);
    }
  }
}