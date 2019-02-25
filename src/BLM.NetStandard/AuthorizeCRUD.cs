﻿using System.Threading.Tasks;
using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTech.BLM.NetStandard
{
    public abstract class AuthorizeCRUD<T> : AuthorizeCollection<T>, IAuthorizeCreate<T>, IAuthorizeModify<T>, IAuthorizeRemove<T> where T : class
    {
        public abstract Task<AuthorizationResult> CanCreateAsync(T entity, IContextInfo ctx);
        public abstract Task<AuthorizationResult> CanModifyAsync(T originalEntity, T modifiedEntity, IContextInfo ctx);

        public abstract Task<AuthorizationResult> CanRemoveAsync(T entity, IContextInfo ctx);

    }
}
