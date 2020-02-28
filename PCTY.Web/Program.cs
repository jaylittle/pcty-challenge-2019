using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace PCTY.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine($"Entry Assembly Location: {System.Reflection.Assembly.GetEntryAssembly().Location}");
      
      var isDevMode = (args != null && args.Contains("--dev", StringComparer.OrdinalIgnoreCase));
      var contentRoot = Directory.GetCurrentDirectory();
      var logRoot = contentRoot + (contentRoot.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())
          ? string.Empty
          : System.IO.Path.DirectorySeparatorChar.ToString())
            + "logs" + System.IO.Path.DirectorySeparatorChar.ToString();

      if (!System.IO.Directory.Exists(logRoot))
      {
        System.IO.Directory.CreateDirectory(logRoot);
      }

      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      var logConfig = new LoggerConfiguration();
      if (environment == Environments.Development)
      {
        logConfig.MinimumLevel.Debug();
      }
      else
      {
        logConfig.MinimumLevel.Information();
      }
      var logger = logConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File($"{logRoot}{System.IO.Path.DirectorySeparatorChar}PCTYLog-.txt", rollingInterval: RollingInterval.Day);
      
      if (isDevMode)
      {
        logger.WriteTo.Console();
      }

      Log.Logger = logger.CreateLogger();

      Log.Information("Configuring Host");
      IHost host = null;
      try
      {
        host = new HostBuilder()
          .UseContentRoot(contentRoot)
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            var env = hostingContext.HostingEnvironment;
            Console.WriteLine($"Hosting Environment: {env.EnvironmentName}");
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", 
                      optional: true, reloadOnChange: false);
            config.AddEnvironmentVariables();
          })
          .ConfigureWebHostDefaults(webHostBuilder => {
            if (isDevMode)
            {
              webHostBuilder = webHostBuilder.UseKestrel();
            }
            else
            {
              webHostBuilder = webHostBuilder.UseIISIntegration();
            }

            webHostBuilder
              .UseStartup<Startup>()
              .UseSerilog()
              .UseDefaultServiceProvider(options => options.ValidateScopes = false);
          })
          .Build();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Host Configuration Failed!");
        throw ex;
      }

      host.Run();
    }
  }
}
