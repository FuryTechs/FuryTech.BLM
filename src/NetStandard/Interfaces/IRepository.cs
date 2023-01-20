using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FuryTechs.BLM.NetStandard.Interfaces
{
    public interface IRepository : IDisposable
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="added"></param>
        /// <param name="contextInfo"></param>
        /// <param name="modified"></param>
        /// <param name="removed"></param>
        /// <param name="isChildRepository"></param>
        /// <returns></returns>
        Task DistributeToListenersAsync(
            List<object> added,
            IContextInfo contextInfo,
            List<Tuple<object, object>> modified,
            List<object> removed,
            bool isChildRepository
        );
    }

    public interface IRepository<T> : IRepository
        where T : class
    {

        /// <summary>
        /// Gets the entity set which is visible to the provided user
        /// </summary>
        /// <param name="user">User who tries to operate with the entity set</param>
        /// <returns>Query object</returns>
        IQueryable<T> Entities(IIdentity user = null);

        /// <summary>
        /// Gets the entity set which is visible to the provided user
        /// </summary>
        /// <param name="user">User who tries to operate with the entity set</param>
        /// <returns>Query object</returns>
        Task<IQueryable<T>> EntitiesAsync(IIdentity user = null);

        /// <summary>
        /// Add a new entity to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void Add(T newItem, IIdentity user = null);

        /// <summary>
        /// Add a new entity to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task AddAsync(T newItem, IIdentity user = null);

        /// <summary>
        /// Add multiple entities to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void AddRange(IEnumerable<T> newItems, IIdentity user = null);

        /// <summary>
        /// Add multiple entities to the repository in the name of the provided user
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task AddRangeAsync(IEnumerable<T> newItems, IIdentity user = null);

        /// <summary>
        /// Removes an entity from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void Remove(T item, IIdentity user = null);

        /// <summary>
        /// Removes an entity from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task RemoveAsync(T item, IIdentity user = null);

        /// <summary>
        /// Removes multiple entities from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        void RemoveRange(IEnumerable<T> items, IIdentity user = null);

        /// <summary>
        /// Removes multiple entities from the repository. The operation will be done by the provided user.
        /// </summary>
        /// <param name="newItem">Entity</param>
        /// <param name="user">(optional) User; When not provided, it will be resolved somehow else</param>
        Task RemoveRangeAsync(IEnumerable<T> items, IIdentity user = null);

        /// <summary>
        /// Resolves the child repository for the given type parameter
        /// </summary>
        /// <typeparam name="T2">Navigation property type</typeparam>
        /// <returns>Child repository</returns>
        IRepository<T2> GetChildRepositoryFor<T2>() where T2 : class;
    }
}
