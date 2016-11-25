namespace BLM
{
    public interface IAuthorizeCreate<T> : IBlmEntry
    {
        /// <summary>
        /// Authorizes an Insert operation
        /// </summary>
        /// <param name="entity">The entity to be inserted</param>
        /// <param name="ctx">The insertion context info</param>
        /// <returns>If the entity can be inserted</returns>
        bool CanCreate(T entity, IContextInfo ctx);

    }
}
