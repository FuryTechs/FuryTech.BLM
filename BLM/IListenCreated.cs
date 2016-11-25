namespace BLM
{
    public interface IListenCreated<T> : IBlmEntry
    {
        /// <summary>
        /// Triggered after the entity is validated, created and saved succesfully in the DB.
        /// </summary>
        /// <param name="entity">The created entity</param>
        /// <param name="context">The creation context</param>
        void OnCreated(T entity, IContextInfo context);
    }
}
