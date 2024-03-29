﻿using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTechs.BLM.NetStandard
{
    public abstract class AuthorizeRemove<T> : IAuthorizeRemove<T>
    {
        public abstract Task<AuthorizationResult> CanRemoveAsync(T entity, IContextInfo ctx);
    }
}