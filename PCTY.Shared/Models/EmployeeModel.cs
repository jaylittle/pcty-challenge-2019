using System;
using System.Collections.Generic;
using PCTY.Shared.Interfaces;

namespace PCTY.Shared.Models
{
  public class EmployeeModel : IPersonModel, IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; } = Guid.Empty;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleInitial { get; set; }
    public double? YearlySalary { get; set; }
    public double? BenefitCost { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
    public List<DependentModel> Dependents = new List<DependentModel>();
  }
}