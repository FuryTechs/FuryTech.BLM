namespace BLM
{
    public interface IAuthorizeRemove<T> : IBlmEntry
    {
        /// <summary>
        /// Authorizes a remove operation
        /// </summary>
        /// <param name="entity">The entity to be removed</param>
        /// <param name="ctx">The remove context info</param>
        /// <returns>If the entity can be removed or not</returns>
        bool CanRemove(T entity, IContextInfo ctx);
    }
}
