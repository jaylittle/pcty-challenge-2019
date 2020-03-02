SELECT
  *
FROM
  Dependent
WHERE
  (@Guid IS NULL OR Guid = @Guid) AND
  (@EmployeeGuid IS NULL OR EmployeeGuid = @EmployeeGuid);