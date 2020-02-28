CREATE TABLE Version
(
  Guid BLOB PRIMARY KEY NOT NULL,
  Major INT NOT NULL,
  Minor INT NOT NULL,
  Build INT NOT NULL,
  CreatedUTC REAL,
  ModifiedUTC REAL
);
CREATE INDEX Version_MajorMinorBuild_index ON Version (Major, Minor, Build);