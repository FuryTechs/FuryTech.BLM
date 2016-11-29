using BLM.Interfaces.Interpret;

namespace BLM
{
    public class Interpret
    {
        public static T BeforeCreate<T>(T entity, IContextInfo context)
        {
            var createInterpreters = Loader.GetEntriesFor<IInterpretBeforeCreate<T>>();
            foreach (var interpreter in createInterpreters)
            {
                var intr = (IInterpretBeforeCreate<T>)interpreter;
                entity = intr.InterpretBeforeCreate(entity, context);
            }
            return entity;
        }

        public static T BeforeModify<T>(T originalEntity, T modifiedEntity, IContextInfo context)
        {
            var modifyInterpreters = Loader.GetEntriesFor<IInterpretBeforeModify<T>>();
            foreach (var interpreter in modifyInterpreters)
            {
                var intr = (IInterpretBeforeModify<T>)interpreter;
                modifiedEntity = intr.InterpretBeforeModify(originalEntity, modifiedEntity, context);
            }
            return modifiedEntity;
        }
    }
}
