using System;
using System.Collections.Generic;
using System.Linq;
using PCTY.Shared.Models;

namespace PCTY.Test.Helpers
{
  public class AssertHelpers
  {
    public static bool ErrorsContain(string errorMessage, OpResult result)
    {
      return result.LogMessages.Any(m => errorMessage.Equals(m.Text, StringComparison.OrdinalIgnoreCase));
    }
  }
}