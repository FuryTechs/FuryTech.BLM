using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FuryTechs.BLM.EntityFrameworkCore
{
    /// <summary>
    /// Interface for EfRepositoeries
    /// </summary>
    public interface IEfRepository : IDisposable
    {
        /// <summary>
        /// Main handler to authorize the entity changes
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="user">User who initiated the operations</param>
        /// <returns></returns>
        Task<AuthorizationResult> AuthorizeEntityChangeAsync(EntityEntry entity, IIdentity user = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="added"></param>
        /// <param name="contextInfo"></param>
        /// <param name="modified"></param>
        /// <param name="removed"></param>
        /// <param name="isChildRepository"></param>
        /// <returns></returns>
        Task DistributeToListenersAsync(
            List<object> added,
            EfContextInfo contextInfo,
            List<Tuple<object, object>> modified,
            List<object> removed,
            bool isChildRepository
        );
    }
}