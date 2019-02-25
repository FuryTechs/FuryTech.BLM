using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTechs.BLM.NetStandard
{
    /// <summary>
    /// Abstract class to authorize user for CRUD operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AuthorizeCRUD<T> : AuthorizeCollection<T>, IAuthorizeCreate<T>, IAuthorizeModify<T>, IAuthorizeRemove<T> where T : class
    {
        /// <summary>
        /// Authorizes user if it can create the entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <param name="ctx">Context information</param>
        /// <returns>Authorization result (deny/accept)</returns>
        public abstract Task<AuthorizationResult> CanCreateAsync(T entity, IContextInfo ctx);

        /// <summary>
        /// Authorizes user if it can update the entity
        /// </summary>
        /// <param name="originalEntity">Original entity</param>
        /// <param name="modifiedEntity">Modified entity</param>
        /// <param name="ctx">Context information</param>
        /// <returns>Authorization result (deny/accept)</returns>
        public abstract Task<AuthorizationResult> CanModifyAsync(T originalEntity, T modifiedEntity, IContextInfo ctx);

        /// <summary>
        /// Authorizes user if it can delete the entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <param name="ctx">Context information</param>
        /// <returns>Authorization result (deny/accept)</returns>
        public abstract Task<AuthorizationResult> CanRemoveAsync(T entity, IContextInfo ctx);

    }
}
