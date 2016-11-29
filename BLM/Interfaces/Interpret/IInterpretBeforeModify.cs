namespace BLM.Interfaces.Interpret
{
    public interface IInterpretBeforeModify<T> : IBlmEntry
    {
        /// <summary>
        /// Possibility to interpret an entity on modification before saving into the DB
        /// </summary>
        /// <param name="originalEntity"></param>
        /// <param name="modifiedEntity"></param>
        /// <param name="context"></param>
        /// <returns>The entity to be modified</returns>
        T InterpretBeforeModify(T originalEntity, T modifiedEntity, IContextInfo context);
    }
}
