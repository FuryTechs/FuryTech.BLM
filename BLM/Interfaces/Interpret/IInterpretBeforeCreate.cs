namespace BLM.Interfaces.Interpret
{
    public interface IInterpretBeforeCreate<T> : IInterpretBeforeCreate<T, T> { }

    public interface IInterpretBeforeCreate<in TInput, out TOutput> : IBlmEntry
    {
        /// <summary>
        /// Possibility to interpret an entity on creation before saving into the DB
        /// </summary>
        /// <param name="entity">The entity to be created</param>
        /// <param name="context">The creation context</param>
        /// <returns>The interpreted entity to be created</returns>
        TOutput InterpretBeforeCreate(TInput entity, IContextInfo context);
    }
}
