using System;

namespace PCTY.Shared.Interfaces
{
  public interface ITimestampModel
  {
    DateTime? CreatedUTC
    {
      get;
      set;
    }
    DateTime? ModifiedUTC
    {
      get;
      set;
    }
  }
}