using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FuryTechs.BLM.EntityFrameworkCore
{
    public interface IEfRepository : IDisposable
    {
        Task<AuthorizationResult> AuthorizeEntityChangeAsync(EntityEntry ent, IIdentity usr = null);

        Task DistributeToListenersAsync(
            List<object> added, 
            EfContextInfo contextInfo,
            List<Tuple<object, object>> modified,
            List<object> removed,
            bool isChildRepository
        );
    }
}