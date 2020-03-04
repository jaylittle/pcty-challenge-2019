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
  public class BenefitCostTests
  {
    private EmployeeBL _employeeBL = null;

    public BenefitCostTests()
    {
      var employeeDalMock = new Mock<IEmployeeDal>();
      _employeeBL = new EmployeeBL(employeeDalMock.Object);
    }

    [Fact]
    public void EmployeeCostChecksOut()
    {
      var empWithNormalName = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000
      };
      Assert.True(empWithNormalName.GetBenefitCosts() == (1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST);

      var empWithSpecialName = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P", YearlySalary = 10000
      };
      Assert.True(empWithSpecialName.GetBenefitCosts() == (1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST);
    }

    [Fact]
    public void DependentCostsChecksOut()
    {
      var empWithNormalNameWithOneStandardDependent = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithNormalNameWithOneStandardDependent.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithNormalNameWithOneSpecialDependent = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithNormalNameWithOneSpecialDependent.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithSpecialNameWithOneStandardDependent = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithSpecialNameWithOneStandardDependent.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithSpecialNameWithOneSpecialDependent = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithSpecialNameWithOneSpecialDependent.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithNormalNameWithTwoStandardDependents = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Joey", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithNormalNameWithTwoStandardDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithSpecialNameWithTwoStandardDependents = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Jane", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Joey", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithSpecialNameWithTwoStandardDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithNormalNameWithMixedDependents = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Jack", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithNormalNameWithMixedDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

       var empWithSpecialNameWithMixedDependents = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Jack", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithSpecialNameWithMixedDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithNormalNameWithTwoSpecialDependents = new EmployeeModel() {
        FirstName = "John", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Anne", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithNormalNameWithTwoSpecialDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.STANDARD_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );

      var empWithSpecialNameWithTwoSpecialDependents = new EmployeeModel() {
        FirstName = "Arthur", LastName = "Doe", MiddleInitial = "P",
        Dependents = new List<DependentModel>() {
          new DependentModel() { FirstName = "Anne", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" },
          new DependentModel() { FirstName = "Art", LastName = "Doe", MiddleInitial = "P", Relationship = "Child" }
        }
      };
      Assert.True(empWithSpecialNameWithTwoSpecialDependents.GetBenefitCosts() ==
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.EMPLOYEE_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST) +
        ((1.00 - PCTY.Logic.BenefitsExtensions.SPECIAL_NAME_DISCOUNT_RATE) * PCTY.Logic.BenefitsExtensions.DEPENDENT_BENEFIT_COST)
      );
    }
  }
}
