UPDATE
  Dependent
SET
  FirstName = @FirstName,
  LastName = @LastName,
  MiddleInitial = @MiddleInitial,
  Relationship = @Relationship,
  ModifiedUTC = @ModifiedUTC
WHERE
  Guid = @Guid AND
  EmployeeGuid = @EmployeeGuid;