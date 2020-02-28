using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using PCTY.Shared;
using PCTY.Shared.Interfaces;
using PCTY.Web.Helpers;
using PCTY.Web.Middleware;
using PCTY.Web.Security;
using PCTY.Data;
using PCTY.Data.Providers;
using PCTY.Data.Interfaces;

namespace PCTY.Web
{
  public class Startup
  {
    private static IHttpContextAccessor _httpContextAccessor;
    public static HttpContext HttpContext
    {
      get
      {
        return _httpContextAccessor.HttpContext;
      }
    }

    public static string ContentRootPath { get; private set; }

    public Startup()
    {
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddMvc(options => {
        options.CacheProfiles.Add("None",
          new CacheProfile()
          {
            Location = ResponseCacheLocation.None,
            NoStore = true
          }
        );
        var contentCacheDuration = 86400;
        if (PCTY.Shared.Settings.Current.CacheControlSeconds > 0)
        {
          contentCacheDuration = PCTY.Shared.Settings.Current.CacheControlSeconds;
        }
        options.CacheProfiles.Add("Content",
          new CacheProfile()
          {
            Location = ResponseCacheLocation.Any,
            Duration = contentCacheDuration,
            NoStore = false
          }
        );
        
        options.EnableEndpointRouting = false;
      }).AddNewtonsoftJson(opt => {
        opt.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
        opt.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
      });

      services.AddScoped<ISettingsProvider, SettingsProvider>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped<IVersionDal, VersionDal>();

      // Add JWT authorization
      var secretKey = PCTY.Shared.Settings.Current.SecretKey.ToString();
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var tokenValidationParameters = new TokenValidationParameters
      {
        // The signing key must match!
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
    
        // Validate the JWT Issuer (iss) claim
        ValidateIssuer = true,
        ValidIssuer = "PCTY",
    
        // Validate the JWT Audience (aud) claim
        ValidateAudience = true,
        ValidAudience = "PCTY",
    
        // Validate the token expiry
        ValidateLifetime = true,
    
        // If you want to allow a certain amount of clock drift, set that here:
        ClockSkew = TimeSpan.Zero,

        // Attempt to prevent token replay attacks
        TokenReplayCache = TokenReplayCache.Instance
      };

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => {
          o.TokenValidationParameters = tokenValidationParameters;
        });

      services.Configure<FormOptions>(options => {
        options.ValueLengthLimit = 1024 * 1024 * 10;
        options.MultipartBodyLengthLimit = 1024 * 1024 * 20;
        options.MultipartHeadersLengthLimit = 1024 * 1024 * 1;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp, ILoggerFactory logFactory)
    {
      _httpContextAccessor = svp.GetRequiredService<IHttpContextAccessor>();
      ContentRootPath = env.ContentRootPath;

      app.UseForwardedHeaders(new ForwardedHeadersOptions()
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      PCTY.Shared.Settings.Startup(env.ContentRootPath);

      // Add Support for JWTs passed in cookies
      app.UseMiddleware<TokenCookieMiddleware>();

      app.UseAuthentication();

      // Add JWT generation
      var secretKey = PCTY.Shared.Settings.Current.SecretKey.ToString();
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var providerOptions = new TokenProviderOptions
      {
        Audience = "PCTY",
        Issuer = "PCTY",
        SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
      };
      app.UseMiddleware<TokenProviderMiddleware>(Options.Create(providerOptions));

      app.Use(async (context, next) =>
      {
        if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
        {
          context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        }
        await next();
      });

      app.Use(async (context, next) => 
      {
        await next();

        var response = context.Response;

        if (response.StatusCode == (int)HttpStatusCode.Unauthorized || response.StatusCode == (int)HttpStatusCode.Forbidden) {
          response.Redirect("/log/in");
        }
      });

      app.UseMvc(m => {
        m.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

      app.UseStaticFiles(new StaticFileOptions() {
        DefaultContentType = "application/octet-stream",
        ServeUnknownFileTypes = true,
        OnPrepareResponse = ctx =>
        {
          var contentCacheDuration = 86400;
          if (PCTY.Shared.Settings.Current.CacheControlSeconds > 0)
          {
            contentCacheDuration = PCTY.Shared.Settings.Current.CacheControlSeconds;
          }
          ctx.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={contentCacheDuration}";
        }
      });

      SystemInfoHelpers.Init(env.ContentRootPath);
      Security.XSRF.Startup(logFactory);
      PCTY.Data.Database.Startup(env.ContentRootPath, new SQLiteDataProvider()).Wait();
    }
  }
}
