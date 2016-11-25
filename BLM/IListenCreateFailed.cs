namespace BLM
{
    public interface IListenCreateFailed<T> : IBlmEntry
    {
        /// <summary>
        /// Triggered, when entity creation fails due validation in the Business Logic
        /// </summary>
        /// <param name="entity">The entity which has failed to create</param>
        /// <param name="context">The creation context</param>
        void OnCreateFailed(T entity, IContextInfo context);
    }
}
