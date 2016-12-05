using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLM.Interfaces.Authorize
{
    internal interface IAuthorizeCollection : IBlmEntry
    {
        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        Task<IQueryable> AuthorizeCollectionAsync(IQueryable entities, IContextInfo ctx);
    }

    internal interface IAuthorizeCollection<in TInput, TOutput> : IAuthorizeCollection
    {
        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        Task<IQueryable<TOutput>> AuthorizeCollectionAsync(IQueryable<TInput> entities, IContextInfo ctx);
    }

    public abstract class AuthorizeCollection<T> : IAuthorizeCollection<T, T>
    {
        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        public abstract Task<IQueryable<T>> AuthorizeCollectionAsync(IQueryable<T> entities, IContextInfo ctx);

        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        public async Task<IQueryable> AuthorizeCollectionAsync(IQueryable entities, IContextInfo ctx)
        {
            return await AuthorizeCollectionAsync(entities.Cast<T>(), ctx);
        }
    }

}
