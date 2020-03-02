UPDATE
  Employee
SET
  FirstName = @FirstName,
  LastName = @LastName,
  MiddleInitial = @MiddleInitial,
  YearlySalary = @YearlySalary,
  BenefitCost = @BenefitCost,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid;