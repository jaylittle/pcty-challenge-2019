using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Routing;
using PCTY.Shared;
using PCTY.Shared.Models;

namespace PCTY.Web.Helpers
{
  public static class Html
  {
    public static HtmlString ContentHashFile(this IHtmlHelper htmlHelper, string webPath
      , bool checkForExistence = false)
    {
      var urlHelper = new UrlHelper(htmlHelper.ViewContext);
      var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", webPath, GetAbsoluteHashPath, checkForExistence).Result;
      string hashUrl = string.Empty;
      if (!checkForExistence || hashEntry != null)
      {
        hashUrl = GetRelativeHashPath(hashEntry.Hash, hashEntry.WebPath);
      }
      return new HtmlString(hashUrl.TrimStart('/'));
    }

    public static string GetRelativeHashPath(string hash, string webPath)
    {
      return $"hash/{hash}/{webPath}";
    }

    public static string GetAbsoluteHashPath(string hash, string webPath)
    {
      return System.IO.Path.Combine(Settings.Current.BasePath, GetRelativeHashPath(hash, webPath));
    }

    public static IHtmlContent RelativeHref(this IHtmlContent htmlContent)
    {
      if (htmlContent is Microsoft.AspNetCore.Mvc.Rendering.TagBuilder)
      {
        var tagBuilder = (Microsoft.AspNetCore.Mvc.Rendering.TagBuilder)htmlContent;
        KeyValuePair<string, string> newHref = default(KeyValuePair<string, string>);
        foreach (var attribute in tagBuilder.Attributes)
        {
          if (attribute.Key.Equals("href", StringComparison.OrdinalIgnoreCase))
          {
            newHref = new KeyValuePair<string, string>(attribute.Key, attribute.Value.TrimStart('/'));
          }
        }
        if (!string.IsNullOrWhiteSpace(newHref.Key))
        {
          tagBuilder.Attributes.Remove(newHref.Key);
          tagBuilder.Attributes.Add(newHref);
        }
        return tagBuilder;
      }
      return htmlContent;
    }

    public static string GetString(this IHtmlContent content)
    {
      var writer = new System.IO.StringWriter();
      content.WriteTo(writer, HtmlEncoder.Default);
      return writer.ToString();
    }
  }
}