using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCTY.Shared.Models;
using PCTY.Shared.Interfaces;
using PCTY.Data.Interfaces;

namespace PCTY.Logic.Interfaces
{
  public interface IEmployeeBL
  {
    Task<IEnumerable<EmployeeModel>> ListEmployees();
    Task<EmployeeModel> GetEmployee(Guid guid);
    Task<OpResult> UpsertEmployee(EmployeeModel record);
    OpResult ValidateEmployee(EmployeeModel record);
    OpResult ValidatePerson(IPersonModel record, string type = "Employee");
  }
}