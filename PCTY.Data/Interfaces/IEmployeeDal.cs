using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCTY.Shared.Models;

namespace PCTY.Data.Interfaces
{
  public interface IEmployeeDal : IBaseDal
  {
    Task<IEnumerable<EmployeeModel>> ListEmployees(Guid? guid = null);
    Task InsertEmployee(EmployeeModel record);
    Task UpdateEmployee(EmployeeModel record);
    Task DeleteEmployee(Guid guid);
    Task<IEnumerable<DependentModel>> ListDependents(Guid? guid = null, Guid? employeeGuid = null);
    Task InsertDependent(DependentModel record);
    Task UpdateDependent(DependentModel record);
    Task DeleteDependent(Guid? guid = null, Guid? employeeGuid = null);
  }
}