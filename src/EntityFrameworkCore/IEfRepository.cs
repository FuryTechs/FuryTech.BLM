using System;
using System.Security.Principal;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard;
using FuryTechs.BLM.NetStandard.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FuryTechs.BLM.EntityFrameworkCore
{
  public interface IEfRepository : IRepository
  {
    /// <summary>
    /// Main handler to authorize the entity changes
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="user">User who initiated the operations</param>
    /// <returns></returns>
    Task<AuthorizationResult> AuthorizeEntityChangeAsync(EntityEntry entity, IIdentity user = null);
  }
}