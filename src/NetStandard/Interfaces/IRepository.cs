using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FuryTechs.BLM.NetStandard.Interfaces
{
    public interface IRepository
    {
        void SaveChanges(IIdentity user);
        Task SaveChangesAsync(IIdentity user);
    }

    public interface IRepository<T> : IRepository<T, T> where T : class
    {
    }

    public interface IRepository<in TInput, out TOutput> : IDisposable, IRepository
        where TInput : class where TOutput : class
    {
        IQueryable<TOutput> Entities(IIdentity user = null);
        void Add(TInput newItem, IIdentity user = null);
        Task AddAsync(TInput newItem, IIdentity user = null);
        void AddRange(IEnumerable<TInput> newItems, IIdentity user = null);
        Task AddRangeAsync(IEnumerable<TInput> newItems, IIdentity user = null);
        void Remove(TInput item, IIdentity user = null);
        Task RemoveAsync(TInput item, IIdentity user = null);
        void RemoveRange(IEnumerable<TInput> items, IIdentity user = null);
        Task RemoveRangeAsync(IEnumerable<TInput> items, IIdentity user = null);
        IRepository<T2> GetChildRepositoryFor<T2>() where T2 : class;
    }
}