using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Claims;
using PCTY.Shared;
using PCTY.Web.Security;
using PCTY.Web.Helpers;

namespace PCTY.Web.Models
{
  public class AppStateModel
  {
    private SettingsData _settings;

    private HttpContext _context;
    private IServiceProvider _svp;
    private string _successUrl;

    public string Url { get; set; }

    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string FullTitle
    {
      get
      {
        if (!string.IsNullOrEmpty(SubTitle) && !Title.Equals(SubTitle, StringComparison.OrdinalIgnoreCase))
        {
          return $"{Title} - {SubTitle}";
        }
        return Title;
      }
    }
    public bool HasAdmin { get; set; }
    public string UserName { get; set; }
    public string UserType { get; set; }
    public string UserFullName { get; set; }
    public double? TokenExpiresRaw { get; set; }
    public DateTime? TokenExpires { get; set; }
    public long? TokenExpiresMilliseconds { get; set; }
    public string VueComponent { get; set; }
    public string RelativeBaseUrl
    {
      get
      {
        return _settings.RelativeBaseUrl.EndsWith('/') ? _settings.RelativeBaseUrl : (_settings.RelativeBaseUrl + '/');
      }
    }
    public string Version { get; set; }
    public string XSRFToken { get; set; }

    public AppStateModel(IServiceProvider svp, SettingsData settings, HttpContext context, string subTitle, string vueComponent, string title = null)
    {
      _settings = settings;
      _context = context;
      SubTitle = subTitle;
      VueComponent = vueComponent;
      _svp = svp;
      Title = title ?? $"PCTY 2020 - Jay Little";

      Init();
    }

    private void Init()
    {
      Url = _context.Request.Path;
      HasAdmin = false;
      UserName = "Anonymous";
      UserType = "Anonymous";
      Version = Helpers.SystemInfoHelpers.Version;

      //Process Authentication
      if (_context.Request?.HttpContext?.User != null && _context.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        HasAdmin = _context.Request.HttpContext.User.IsInRole("PCTYAdmin");
        UserName = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("UserName"))?.Value;
        UserType = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("UserType"))?.Value;
        string sessionId = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("SessionId"))?.Value;
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TokenExpires = epoch.AddSeconds(double.Parse(_context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("exp"))?.Value));
        TokenExpiresMilliseconds = (long)TokenExpires.Value.Subtract(DateTime.UtcNow).TotalMilliseconds;
        XSRFToken = Security.XSRF.GetXsrfValues(UserName, sessionId).formValue;
      }

      if (!string.IsNullOrWhiteSpace(_context.Request.Query["successUrl"]))
      {
        _successUrl = _context.Request.Query["successUrl"];
      }
      else
      {
        _successUrl = null;
      }
    }

    public string Json()
    {
      return JsonConvert.SerializeObject(this, new JsonSerializerSettings 
      { 
        ContractResolver = new CamelCasePropertyNamesContractResolver() 
      });
    }
  }
}
