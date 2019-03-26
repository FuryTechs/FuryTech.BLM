using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FuryTechs.BLM.NetStandard.Interfaces
{
    public interface IContextInfo<T>
    {
        /// <summary>
        /// Exposes the type of the entity. 
        /// If you use interfaces, you can check which type of entity fetch is in progress.
        /// </summary>
        Type EntityType { get; }
    }

    public interface IContextInfo
    {
        /// <summary>
        /// The user identity
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// Get full the full queryable entity set
        /// </summary>
        /// <typeparam name="T">The entity type parameter</typeparam>
        /// <returns>The full queryable entity set</returns>
        IQueryable<T> GetFullEntitySet<T>() where T : class;

        /// <summary>
        /// Get full the authorized queryable entity set
        /// </summary>
        /// <typeparam name="T">The entity type parameter</typeparam>
        /// <returns>The authorized queryable entity set</returns>
        Task<IQueryable<T>> GetAuthorizedEntitySetAsync<T>() where T : class;

    }
}
