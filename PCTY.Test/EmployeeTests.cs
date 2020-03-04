using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using PCTY.Shared.Models;
using PCTY.Data.Interfaces;
using PCTY.Logic.Interfaces;
using PCTY.Logic;
using PCTY.Test.Helpers;
using Moq;

namespace PCTY.Test
{
  public class EmployeeTests
  {
    private EmployeeBL _employeeBL = null;

    public EmployeeTests()
    {
      var employeeDalMock = new Mock<IEmployeeDal>();
      _employeeBL = new EmployeeBL(employeeDalMock.Object);
    }

    [Fact]
    public void EmployeeValidationChecksOut()
    {
      var empMissingFirstName = new EmployeeModel() {
        LastName = "Doe", MiddleInitial = "P"
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_FIRST_NAME_REQUIRED, EmployeeBL.TYPE_EMPLOYEE)
        , _employeeBL.ValidateEmployee(empMissingFirstName)));

      
      var empMissingLastName = new EmployeeModel() {
        FirstName = "John", MiddleInitial = "P"
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_LAST_NAME_REQUIRED, EmployeeBL.TYPE_EMPLOYEE)
        , _employeeBL.ValidateEmployee(empMissingLastName)));

      var empYearlySalaryLessThanZero = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = -1
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_YEARLY_SALARY_AT_LEAST_ZERO, EmployeeBL.TYPE_EMPLOYEE)
        , _employeeBL.ValidateEmployee(empYearlySalaryLessThanZero)));

      var empPerfect = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000
      };
      Assert.False(_employeeBL.ValidateEmployee(empPerfect).LogMessages.Any());
    }

    [Fact]
    public void DependentValidationChecksOut()
    {
      var empDependentMissingFirstName = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000,
        Dependents = new List<DependentModel>() {
          new DependentModel() { LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_FIRST_NAME_REQUIRED, string.Format(EmployeeBL.TYPE_DEPENDENT, 1))
        , _employeeBL.ValidateEmployee(empDependentMissingFirstName)));

      var empDependentMissingLastName = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000,
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_LAST_NAME_REQUIRED, string.Format(EmployeeBL.TYPE_DEPENDENT, 1))
        , _employeeBL.ValidateEmployee(empDependentMissingLastName)));

      var empDependentMissingRelationship = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000,
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P" }
        }
      };
      Assert.True(AssertHelpers.ErrorsContain(string.Format(EmployeeBL.ERROR_RELATIONSHIP_REQUIRED, string.Format(EmployeeBL.TYPE_DEPENDENT, 1))
        , _employeeBL.ValidateEmployee(empDependentMissingRelationship)));

      var empDependentPerfect = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000,
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.False(_employeeBL.ValidateEmployee(empDependentPerfect).LogMessages.Any());
    }
  }
}
