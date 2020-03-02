CREATE TABLE Employee
(
  Guid BLOB PRIMARY KEY NOT NULL,
  FirstName TEXT NOT NULL,
  LastName TEXT NOT NULL,
  MiddleInitial TEXT NOT NULL,
  YearlySalary NUMBER NOT NULL,
  BenefitCost NUMBER NOT NULL,
  CreatedUTC REAL NOT NULL,
  ModifiedUTC REAL NOT NULL
);
CREATE INDEX Employee_FirstLastMiddle_index ON Employee (FirstName, LastName, MiddleInitial);

CREATE TABLE Dependent
(
  Guid BLOB PRIMARY KEY NOT NULL,
  EmployeeGuid BLOB NOT NULL, 
  FirstName TEXT NOT NULL,
  LastName TEXT NOT NULL,
  MiddleInitial TEXT NOT NULL,
  Relationship TEXT NOT NULL,
  CreatedUTC REAL NOT NULL,
  ModifiedUTC REAL NOT NULL,
  FOREIGN KEY (EmployeeGuid) REFERENCES Employee (Guid)
);
CREATE INDEX Dependent_EmployeeFirstLastMiddle_index ON Dependent (EmployeeGuid, FirstName, LastName, MiddleInitial);