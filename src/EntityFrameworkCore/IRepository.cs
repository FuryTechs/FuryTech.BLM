using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuryTechs.BLM.EntityFrameworkCore
{
  public interface IRepository<T, TDbContext> : IRepository<T>
    where T : class
    where TDbContext : DbContext
  {
  }
}