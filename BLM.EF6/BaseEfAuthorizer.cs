using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using BLM.Authorization;

namespace BLM.EF6
{
    public abstract class BaseEfAuthorizer<T> : IAuthorizer<T>
    {
        public DbContext DbContext { get; set; }

        public abstract bool CanInsert(IIdentity usr, T entity);

        public abstract bool CanUpdate(IIdentity usr, T originalEntity, T modifiedEntity);

        public abstract bool CanRemove(IIdentity usr, T entity);


        public abstract IQueryable<T> AuthorizeCollection(IIdentity usr, IQueryable<T> entities);

    }
}
