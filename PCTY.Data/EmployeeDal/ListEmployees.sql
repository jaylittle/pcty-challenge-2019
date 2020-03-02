SELECT
  *
FROM
  Employee
WHERE
  (@Guid IS NULL OR Guid = @Guid);