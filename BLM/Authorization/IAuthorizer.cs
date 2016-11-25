using System.Linq;

namespace BLM.Authorization
{
    public interface IAuthorizer<T>
    {
        bool CanInsert(T entity, IContextInfo ctx);
        bool CanUpdate(T originalEntity, T modifiedEntity, IContextInfo ctx);
        bool CanRemove(T entity, IContextInfo ctx);
        IQueryable<T> AuthorizeCollection(IQueryable<T> entities, IContextInfo ctx);
    }
}
