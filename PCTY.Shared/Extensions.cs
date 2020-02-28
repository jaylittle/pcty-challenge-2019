using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared
{
  public static class Extensions
  {
    public static void UpdateTimestamps(this ITimestampModel record, bool newRecord = false, bool importFlag = false)
    {
      if (!importFlag)
      {
        var currentTime = DateTime.UtcNow;
        if (newRecord)
        {
          record.CreatedUTC = currentTime;
        }
        record.ModifiedUTC = currentTime;
      }
    }

    public static void UpdateGuid(this IGuidModel record)
    {
      if (Guid.Empty.Equals(record.Guid))
      {
        record.Guid = Guid.NewGuid();
      }
    }
    
    public static bool IsUrlAbsolute(this string url)
    {
      return Helpers.IsUrlAbsolute(url);
    }
  }
}