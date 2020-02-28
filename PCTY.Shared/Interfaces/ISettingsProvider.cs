using System;

namespace PCTY.Shared.Interfaces
{
  public interface ISettingsProvider
  {
    SettingsData Current { get; set; }
  }
}