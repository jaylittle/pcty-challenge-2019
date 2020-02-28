using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared
{
  public class SettingsData
  {
    public int TimeLimitAdminToken { get; set; } = 30;
    public virtual string Password { get; set; } = string.Empty;
    private string _externalBaseUrl = "http://localhost:5000/";
    public string ExternalBaseUrl
    {
      get
      {
        return _externalBaseUrl;
      }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
        {
          _externalBaseUrl = "/";
        }
        _externalBaseUrl = value.EndsWith("/") ? value : $"{value}/";
      }
    }
    private string _relativeBaseUrl = "/";
    public string RelativeBaseUrl
    {
      get
      {
        return _relativeBaseUrl;
      }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
        {
          _relativeBaseUrl = "/";
        }
        _relativeBaseUrl = value.EndsWith("/") ? value : $"{value}/";
      }
    }
    public virtual Guid SecretKey { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = "Tester";
    public string CookieDomain { get; set; } = string.Empty;
    public string BasePath { get; set; } = "/";
    public int CacheControlSeconds { get; set; } = 86400;

    [JsonIgnore]
    public string CookiePath
    {
      get
      {
        if (string.IsNullOrWhiteSpace(BasePath))
        {
          return string.Empty;
        }
        return BasePath.TrimEnd('/');
      }
    }

    public SettingsData()
    {
      Password = PCTY.Shared.Security.HashWithKey("Tester", SecretKey);
    }
  }

  public class SettingsProvider : ISettingsProvider
  {
    public SettingsData Current
    {
      get
      {
        return Settings.Current;
      }
      set
      {
        Settings.Current = value;
      }
    }
  }

  public class Settings
  {
    public static void Startup(string contentRootPath)
    {
      ContentRootPath = contentRootPath;
      var current = Current;
      if (_defaultSettings)
      {
        Console.WriteLine("Default Settings detected - writing new settings file to disk");
        Current = _current;
      }
    }

    private static bool _defaultSettings = true;
    private static SettingsData _current;
    public static SettingsData Current
    {
      get
      {
        if (_current == null)
        {
          if (System.IO.File.Exists(SettingsFilePath))
          {
            try
            {
              _current = JsonConvert.DeserializeObject<SettingsData>(System.IO.File.ReadAllText(SettingsFilePath));
              _defaultSettings = false;
            }
            catch (Exception ex)
            {
              //JSON file is fucked - log to console and move the fuck on
              Console.WriteLine($"JSON Settings Read Failed with Error: {ex.Message}");
              _current = null;
            }
          }
          if (_current == null)
          {
            _defaultSettings = true;
            _current = new SettingsData();
          }        
        }
        return _current;
      }
      set
      {
        if (System.IO.File.Exists(SettingsFilePath))
        {
          System.IO.File.Delete(SettingsFilePath);
        }
        System.IO.File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(_current, new JsonSerializerSettings(){
          ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
          Formatting = Formatting.Indented
        }));
        _defaultSettings = false;
      }
    }

    private static string SettingsFilePath
    {
      get 
      {
        return $"{ContentRootPath}pcty.settings.json";
      }
    }

    private static string _contentRootPath;
    public static string ContentRootPath
    {
      get
      {
        return _contentRootPath;
      }
      private set 
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? string.Empty : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }
  }
}