INSERT INTO Dependent (
  Guid,
  EmployeeGuid,
  FirstName,
  LastName,
  MiddleInitial,
  Relationship,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @EmployeeGuid,
  @FirstName,
  @LastName,
  @MiddleInitial,
  @Relationship,
  @CreatedUTC,
  @ModifiedUTC
);