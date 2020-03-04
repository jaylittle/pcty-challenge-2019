using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCTY.Shared;
using PCTY.Shared.Interfaces;
using PCTY.Shared.Models;
using PCTY.Data;
using PCTY.Data.Interfaces;
using PCTY.Logic.Interfaces;

namespace PCTY.Logic
{
  public class EmployeeBL : IEmployeeBL
  {
    private IEmployeeDal _employeeDal;

    public EmployeeBL(IEmployeeDal employeeDal)
    {
      _employeeDal = employeeDal;
    }

    public async Task<IEnumerable<EmployeeModel>> ListEmployees()
    {
      return await _employeeDal.ListEmployees();
    }

    public async Task<EmployeeModel> GetEmployee(Guid guid)
    {
      var employeeTask = _employeeDal.ListEmployees(guid);
      var dependentTask = _employeeDal.ListDependents(null, guid);

      await Task.WhenAll(employeeTask, dependentTask);;

      var employee = employeeTask.Result.FirstOrDefault();
      if (employee != null)
      {
        employee.Dependents = dependentTask.Result.ToList();
      }
      return employee;
    }

    public async Task<OpResult> UpsertEmployee(EmployeeModel record)
    {
      var existingRecord = await GetEmployee(record.Guid);
      var retvalue = ValidateEmployee(record);
      if (retvalue.Successful)
      {
        record.Dependents = record.Dependents ?? new List<DependentModel>();

        record.UpdateGuid();
        record.UpdateTimestamps(existingRecord == null);
        record.BenefitCost = record.GetBenefitCosts();

        _employeeDal.OpenTransaction(DatabaseType.PCTY, false);
        try
        {
          //Update Employee
          if (existingRecord == null)
          {
            await _employeeDal.InsertEmployee(record);
          }
          else
          {
            await _employeeDal.UpdateEmployee(record);
          }

          //Update Dependents
          var dependentTasks = new List<Task>();
          foreach (var dependent in record.Dependents)
          {
            dependent.UpdateTimestamps(Guid.Empty.Equals(dependent.Guid));
            dependent.UpdateGuid();
            dependent.EmployeeGuid = record.Guid;
          }
          var existingDependents = new Dictionary<Guid, DependentModel>();
          if (existingRecord != null && existingRecord.Dependents != null)
          {
            existingDependents = existingRecord.Dependents.ToDictionary(d => d.Guid, d => d);
          }
          var newDependents = record.Dependents.Where(d => !Guid.Empty.Equals(d.Guid)).ToDictionary(d => d.Guid, d => d);

          var deletedDependents = existingDependents.Where(ed => !newDependents.ContainsKey(ed.Key));
          foreach (var deletedDependent in deletedDependents)
          {
            dependentTasks.Add(_employeeDal.DeleteDependent(deletedDependent.Value.Guid, deletedDependent.Value.EmployeeGuid));
          }
          foreach (var newDependent in record.Dependents)
          {
            if (existingDependents.ContainsKey(newDependent.Guid))
            {
              dependentTasks.Add(_employeeDal.UpdateDependent(newDependent));
            }
            else
            {
              dependentTasks.Add(_employeeDal.InsertDependent(newDependent));
            }
          }
          await Task.WhenAll(dependentTasks.ToArray());

          _employeeDal.CommitTransaction(DatabaseType.PCTY);
        }
        catch (Exception ex)
        {
          _employeeDal.RollBackTransaction(DatabaseType.PCTY);
          throw ex;
        }
      }
      return retvalue;
    }

    public const string TYPE_EMPLOYEE = "Employee";
    public const string TYPE_DEPENDENT = "Dependent {0}";
    public const string ERROR_YEARLY_SALARY_AT_LEAST_ZERO = "{0} Yearly Salary must be at least zero!";
    public const string ERROR_FIRST_NAME_REQUIRED = "{0} First Name is required!";
    public const string ERROR_LAST_NAME_REQUIRED = "{0} Last Name is required!";
    public const string ERROR_RELATIONSHIP_REQUIRED = "{0} Relationship is required!";

    public OpResult ValidateEmployee(EmployeeModel record)
    {
      var retvalue = new OpResult();
      retvalue.Inhale(ValidatePerson(record, TYPE_EMPLOYEE));
      if (record.YearlySalary < 0)
      {
        retvalue.LogError(string.Format(ERROR_YEARLY_SALARY_AT_LEAST_ZERO, TYPE_EMPLOYEE));
      }

      if (record.Dependents != null && record.Dependents.Any())
      {
        var dependentCounter = 1;
        foreach  (var dependent in record.Dependents)
        {
          var personType = string.Format(TYPE_DEPENDENT, dependentCounter);
          retvalue.Inhale(ValidatePerson(dependent, personType));
          if (string.IsNullOrWhiteSpace(dependent.Relationship))
          {
            retvalue.LogError(string.Format(ERROR_RELATIONSHIP_REQUIRED, personType));
          }
          dependentCounter++;
        }
      }
      return retvalue;
    }

    public OpResult ValidatePerson(IPersonModel record, string type)
    {
      var retvalue = new OpResult();
      if (string.IsNullOrWhiteSpace(record.FirstName))
      {
        retvalue.LogError(string.Format(ERROR_FIRST_NAME_REQUIRED, type));
      }
      if (string.IsNullOrWhiteSpace(record.LastName))
      {
        retvalue.LogError(string.Format(ERROR_LAST_NAME_REQUIRED, type));
      }
      return retvalue;
    }
  }
}