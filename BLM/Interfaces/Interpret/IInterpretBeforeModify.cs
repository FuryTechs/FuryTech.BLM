namespace BLM.Interfaces.Interpret
{
    public interface IInterpretBeforeModify<T> : IInterpretBeforeModify<T, T>
    {

    }

    public interface IInterpretBeforeModify<in TInput, out TOutput> : IBlmEntry
    {
        /// <summary>
        /// Possibility to interpret an entity on modification before saving into the DB
        /// </summary>
        /// <param name="originalEntity"></param>
        /// <param name="modifiedEntity"></param>
        /// <param name="context"></param>
        /// <returns>The entity to be modified</returns>
        TOutput InterpretBeforeModify(TInput originalEntity, TInput modifiedEntity, IContextInfo context);
    }
}
