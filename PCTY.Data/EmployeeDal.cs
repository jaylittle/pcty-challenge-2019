using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PCTY.Shared;
using PCTY.Shared.Models;
using PCTY.Data;
using PCTY.Data.Interfaces;

namespace PCTY.Data
{
  public class EmployeeDal : BaseDal<EmployeeDal>, IEmployeeDal
  {
    public async Task<IEnumerable<EmployeeModel>> ListEmployees(Guid? guid = null)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, true))
      {
        return await ct.DbConnection.QueryAsync<EmployeeModel>(ReadQuery("ListEmployees", ct.ProviderName), param: new {
          Guid = guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertEmployee(EmployeeModel record)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertEmployee", ct.ProviderName), param: record);
      }
    }

    public async Task UpdateEmployee(EmployeeModel record)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateEmployee", ct.ProviderName), param: record);
      }
    }

    public async Task DeleteEmployee(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteEmployee", ct.ProviderName), param: new {
          Guid = guid
        });
      }
    }

    public async Task<IEnumerable<DependentModel>> ListDependents(Guid? guid = null, Guid? employeeGuid = null)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, true))
      {
        return await ct.DbConnection.QueryAsync<DependentModel>(ReadQuery("ListDependents", ct.ProviderName), param: new {
          Guid = guid,
          EmployeeGuid = employeeGuid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertDependent(DependentModel record)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertDependent", ct.ProviderName), param: record);
      }
    }

    public async Task UpdateDependent(DependentModel record)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateDependent", ct.ProviderName), param: record);
      }
    }

    public async Task DeleteDependent(Guid? guid = null, Guid? employeeGuid = null)
    {
      using (var ct = GetConnection(DatabaseType.PCTY, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteDependent", ct.ProviderName), param: new {
          Guid = guid,
          EmployeeGuid = employeeGuid
        });
      }
    }
  }
}
