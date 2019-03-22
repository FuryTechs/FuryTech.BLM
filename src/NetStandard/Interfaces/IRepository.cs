using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FuryTechs.BLM.NetStandard.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// Saves the changes
        /// </summary>
        /// <param name="user"></param>
        void SaveChanges(IIdentity user = null);

        /// <summary>
        /// Save the changes asynchronously
        /// </summary>
        /// <param name="user">User who has done the changes</param>
        Task SaveChangesAsync(IIdentity user = null);
    }

    public interface IRepository<T> : IRepository<T, T> where T : class
    {
    }

    public interface IRepository<in TInput, out TOutput> : IDisposable, IRepository
        where TInput : class where TOutput : class
    {
        /// <summary>
        /// Gets the entity set which is visible to the provided user
        /// </summary>
        /// <param name="user">User who tries to operate with the entity set</param>
        /// <returns>Query object</returns>
        IQueryable<TOutput> Entities(IIdentity user = null);

        /// <summary>
        /// Add a new entity to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void Add(TInput newItem, IIdentity user = null);

        /// <summary>
        /// Add a new entity to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task AddAsync(TInput newItem, IIdentity user = null);

        /// <summary>
        /// Add multiple entities to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void AddRange(IEnumerable<TInput> newItems, IIdentity user = null);

        /// <summary>
        /// Add multiple entities to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task AddRangeAsync(IEnumerable<TInput> newItems, IIdentity user = null);

        /// <summary>
        /// Removes an entity from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void Remove(TInput item, IIdentity user = null);
        
        /// <summary>
        /// Removes an entity from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task RemoveAsync(TInput item, IIdentity user = null);

        /// <summary>
        /// Removes multiple entities from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void RemoveRange(IEnumerable<TInput> items, IIdentity user = null);

        /// <summary>
        /// Removes multiple entities from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task RemoveRangeAsync(IEnumerable<TInput> items, IIdentity user = null);

        /// <summary>
        /// Resolves the child repository for the given type parameter
        /// </summary>
        /// <typeparam name="T2">Navigation property type</typeparam>
        /// <returns>Child repository</returns>
        IRepository<T2> GetChildRepositoryFor<T2>() where T2 : class;
    }
}