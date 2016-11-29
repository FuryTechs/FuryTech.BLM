using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BLM
{
    public interface IRepository<T> : IDisposable where T: class
    {
        IQueryable<T> Entities(IIdentity user);
        Task AddAsync(IIdentity user, T newItem);
        Task AddRangeAsync(IIdentity user, IEnumerable<T> newItems);
        Task RemoveAsync(IIdentity usr, T item);
        Task RemoveRangeAsync(IIdentity usr, IEnumerable<T> items);
        Task SaveChangesAsync(IIdentity user);
    }

}
