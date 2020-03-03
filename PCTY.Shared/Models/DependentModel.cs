using System;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared.Models
{
  public class DependentModel : IPersonModel, IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; } = Guid.Empty;
    public Guid EmployeeGuid { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleInitial { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}