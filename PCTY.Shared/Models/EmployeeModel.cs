using System;
using System.Collections.Generic;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared.Models
{
  public class EmployeeModel : IPersonModel, IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleInitial { get; set; } = string.Empty;
    public double YearlySalary { get; set; } = 0;
    public double BenefitCost { get; set; } = 0;
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
    public List<DependentModel> Dependents = new List<DependentModel>();
  }
}