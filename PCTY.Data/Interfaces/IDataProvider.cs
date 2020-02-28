using System;
using System.Collections.Generic;
using System.Data.Common;
using PCTY.Data;

namespace PCTY.Data.Interfaces
{
  public interface IDataProvider
  {
    string Name { get; }
    bool RequiresFolder { get; }
    bool SingleWrite { get; }
    void Init(string databaseFolderPath);
    DbConnection GetConnection(DatabaseType type, bool readOnly = true);
    DbTransaction GetTransaction(DatabaseType type, bool readOnly = false);
  }
}