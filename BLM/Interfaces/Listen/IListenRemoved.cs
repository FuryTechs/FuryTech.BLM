using System.Threading.Tasks;

namespace BLM.Interfaces.Listen
{
    public interface IListenRemoved<T> : IBlmEntry
    {
        /// <summary>
        /// Triggered after successfully validating and removing an entity.
        /// </summary>
        /// <param name="entity">The removed entity (with original properties).</param>
        /// <param name="}">The context info.</param>
        Task OnRemovedAsync(T entity, IContextInfo context);
    }
}
