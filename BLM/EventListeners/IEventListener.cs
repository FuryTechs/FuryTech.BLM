using System.Security.Principal;

namespace BLM.EventListeners
{
    public interface IEventListener<T> : IEventListener
    {
        /// <summary>
        /// Possibility to interpret an entity on creation before saving into the DB
        /// </summary>
        /// <param name="entity">The entity to be created</param>
        /// <param name="user">The creator user</param>
        /// <returns>The entity to be created</returns>
        T OnBeforeCreate(T entity, IIdentity user);

        /// <summary>
        /// Possibility to interpret an entity on modification before saving into the DB
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns>The entity to be modified</returns>
        T OnBeforeModify(T originalEntity, T modifiedEntity, IIdentity user);

        /// <summary>
        /// Triggered after the entity is validated, created and saved succesfully in the DB.
        /// </summary>
        /// <param name="entity">The created entity</param>
        /// <param name="user">The user who created the entity</param>
        void OnCreated(T entity, IIdentity user);


        /// <summary>
        /// Triggered, when entity creation fails due validation in the Business Logic
        /// </summary>
        /// <param name="entity">The entity which has failed to create</param>
        /// <param name="user">The user who tried to create the entity</param>
        void OnCreationValidationFailed(T entity, IIdentity user);

        /// <summary>
        /// Triggered after an already existing entity is modified, validated and saved succesfully in the DB.
        /// </summary>
        /// <param name="originalEntity">The DbEntityEntry for the update.</param>
        /// <param name="modifiedEntity">The succesfully modified entity</param>
        /// <param name="user">The user who modified the entity.</param>
        void OnModified(T originalEntity, T modifiedEntity, IIdentity user);

        /// <summary>
        /// Triggered when an entity modification has been failed due validation in the Business Layer
        /// </summary>
        /// <param name="originalEntity">The DbEntityEntry for the update.</param>
        /// <param name="modifiedEntity">The entity which has been failed to modify</param>
        /// <param name="user">The user who tried to modify the entity</param>
        void OnModificationFailed(T originalEntity, T modifiedEntity, IIdentity user);

        /// <summary>
        /// Triggered after successfully validating and removing an entity.
        /// </summary>
        /// <param name="entity">The removed entity (with original properties).</param>
        /// <param name="user">The user who removed the entity.</param>
        void OnRemoved(T entity, IIdentity user);

        /// <summary>
        /// Triggered when an entity remove failed due validation in the Business Layer
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        void OnRemoveFailed(T entity, IIdentity user);
    }

    public interface IEventListener { }
}
