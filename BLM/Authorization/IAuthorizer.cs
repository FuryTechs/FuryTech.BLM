using System;
using System.Linq;
using System.Security.Principal;

namespace BLM.Authorization
{
    public interface IAuthorizer<T>
    {
        bool CanInsert(IIdentity usr, T entity);
        bool CanUpdate(IIdentity usr, T originalEntity, T modifiedEntity);
        bool CanRemove(IIdentity usr, T entity);
        IQueryable<T> AuthorizeCollection(IIdentity usr, IQueryable<T> entities);
    }
}
