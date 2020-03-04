DELETE FROM
  [Dependent]
WHERE
  EmployeeGuid = @Guid;

DELETE FROM
  Employee
WHERE
  Guid = @Guid;