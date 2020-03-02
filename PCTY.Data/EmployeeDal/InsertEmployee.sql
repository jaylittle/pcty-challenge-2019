INSERT INTO Employee (
  Guid,
  FirstName,
  LastName,
  MiddleInitial,
  YearlySalary,
  BenefitCost,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @FirstName,
  @LastName,
  @MiddleInitial,
  @YearlySalary,
  @BenefitCost,
  @CreatedUTC,
  @ModifiedUTC
);