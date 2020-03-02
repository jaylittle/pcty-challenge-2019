using System;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared.Models
{
  public class DependentModel : IPersonModel, IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; } = Guid.Empty;
    public Guid EmployeeGuid { get; set; } = Guid.Empty;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleInitial { get; set; }
    public string Relationship { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}