using System.Linq;

namespace BLM
{
    public interface IAuthorizeCollection<T> : IBlmEntry
    {
        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        IQueryable<T> AuthorizeCollection(IQueryable<T> entities, IContextInfo ctx);
    }
}
