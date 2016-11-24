using BLM.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace BLM.Repository
{
    public interface IRepository<T> : IDisposable where T: class
    {
        IAuthorizer<T> Authorizer { get; }
        IQueryable<T> Entities(IIdentity user);
        void Add(IIdentity user, T newItem);
        void AddRange(IIdentity user, IEnumerable<T> newItems);
        void Remove(IIdentity usr, T item);
        void RemoveRange(IIdentity usr, IEnumerable<T> items);
        void SaveChanges(IIdentity user);
    }

}
