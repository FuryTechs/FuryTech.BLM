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
        void Add(IIdentity user, T newItem);
        Task AddAsync(IIdentity user, T newItem);
        void AddRange(IIdentity user, IEnumerable<T> newItems);
        Task AddRangeAsync(IIdentity user, IEnumerable<T> newItems);
        void Remove(IIdentity usr, T item);
        Task RemoveAsync(IIdentity usr, T item);
        void RemoveRange(IIdentity usr, IEnumerable<T> items);
        Task RemoveRangeAsync(IIdentity usr, IEnumerable<T> items);
        void SaveChanges(IIdentity user);
        Task SaveChangesAsync(IIdentity user);
    }

}
