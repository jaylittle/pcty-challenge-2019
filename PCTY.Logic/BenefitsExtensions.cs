using System;
using System.Collections.Generic;
using System.Linq;
using PCTY.Shared.Models;
using PCTY.Shared.Interfaces;

namespace PCTY.Logic
{
  public static class BenefitsExtensions
  {
    public const double EMPLOYEE_BENEFIT_COST = 1000;
    public const double DEPENDENT_BENEFIT_COST = 500;
    public const double SPECIAL_NAME_DISCOUNT_RATE = 0.10;
    public const double STANDARD_NAME_DISCOUNT_RATE = 0.00;
    public static double GetBenefitsDiscountRate(this IPersonModel person)
    {
      if ((person?.FirstName ?? string.Empty).StartsWith("A", StringComparison.OrdinalIgnoreCase))
      {
        return SPECIAL_NAME_DISCOUNT_RATE;
      }
      return STANDARD_NAME_DISCOUNT_RATE;
    }

    public static double GetBenefitCosts(this EmployeeModel employee)
    {
      double benefitCost = (1.00 - employee.GetBenefitsDiscountRate()) * EMPLOYEE_BENEFIT_COST;
      if (employee.Dependents != null && employee.Dependents.Count > 0)
      {
        benefitCost += employee.Dependents.Sum(d => d.CalculateBenefitCosts());
      }
      return benefitCost;
    }

    public static double CalculateBenefitCosts(this DependentModel dependent)
    {
      var benefitCost = (1.00 - dependent.GetBenefitsDiscountRate()) * DEPENDENT_BENEFIT_COST;
      return benefitCost;
    }
  }
}
