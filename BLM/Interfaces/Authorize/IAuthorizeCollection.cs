using System.Linq;
using System.Threading.Tasks;

namespace BLM.Interfaces.Authorize
{
    public interface IAuthorizeCollection<T> : IBlmEntry
    {
        /// <summary>
        /// Authorizes a collection to be read
        /// </summary>
        /// <param name="entities">The full entity set</param>
        /// <param name="ctx">The collection context info</param>
        /// <returns>The authorized entities</returns>
        Task<IQueryable<T>> AuthorizeCollectionAsync(IQueryable<T> entities, IContextInfo ctx);
    }
}
