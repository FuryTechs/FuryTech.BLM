namespace BLM
{
    public interface IListenRemoved<T> : IBlmEntry
    {
        /// <summary>
        /// Triggered after successfully validating and removing an entity.
        /// </summary>
        /// <param name="entity">The removed entity (with original properties).</param>
        /// <param name="}">The context info.</param>
        void OnRemoved(T entity, IContextInfo context);
    }
}
