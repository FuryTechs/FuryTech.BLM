namespace BLM
{
    public interface IListenRemoveFailed<T> : IBlmEntry
    {
        /// <summary>
        /// Triggered when an entity remove failed due validation in the Business Layer
        /// </summary>
        /// <param name="entity">The entity which has been failed to remove</param>
        /// <param name="context">The removal context</param>
        void OnRemoveFailed(T entity, IContextInfo context);
    }
}
