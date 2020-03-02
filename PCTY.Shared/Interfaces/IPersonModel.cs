using System;

namespace PCTY.Shared.Interfaces
{
  public interface IPersonModel
  {
    string FirstName { get; set; }
    string LastName { get; set; }
    string MiddleInitial { get; set; }
  }
}