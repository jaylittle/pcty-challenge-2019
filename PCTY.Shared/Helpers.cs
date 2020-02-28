using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared
{
  public class Helpers
  {
    public static bool IsUrlAbsolute(string url)
    {
      Uri outUri;
      return !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out outUri)
        && !url.StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase)
        && !(new string[] { "file" }).Any(s => s.Equals(outUri.Scheme, StringComparison.OrdinalIgnoreCase));
    }
  }
}