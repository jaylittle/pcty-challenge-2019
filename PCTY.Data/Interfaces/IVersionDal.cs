using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCTY.Shared.Models;
using PCTY.Data;

namespace PCTY.Data.Interfaces
{
  public interface IVersionDal : IBaseDal
  {
    Task<IEnumerable<VersionModel>> ListVersions(DatabaseType type);
    Task<VersionModel> GetCurrentVersion(DatabaseType type);
    Task InsertVersion(DatabaseType type, VersionModel version);
  }
}