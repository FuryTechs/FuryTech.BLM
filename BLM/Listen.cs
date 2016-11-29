using System.Threading.Tasks;
using BLM.Interfaces.Listen;

namespace BLM
{
    public class Listen
    {
        public static async Task Created<T>(T entity, IContextInfo context)
        {
            var createListeners = Loader.GetEntriesFor<IListenCreated<T>>();
            foreach (var createListener in createListeners)
            {
                await ((dynamic)createListener).OnCreatedAsync(entity, context);
            }
        }

        public static async Task CreateFailed<T>(T entity, IContextInfo context)
        {
            var createFailListeners = Loader.GetEntriesFor<IListenCreateFailed<T>>();
            foreach (var listener in createFailListeners)
            {
                await ((dynamic)listener).OnCreateFailedAsync(entity, context);
            }
        }

        public static async Task Modified<T>(T original, T modified, IContextInfo context)
        {

            var modifyListeners = Loader.GetEntriesFor<IListenModified<T>>();
            foreach (var listener in modifyListeners)
            {
                await ((dynamic)listener).OnModifiedAsync(original, modified, context);
            }
        }

        public static async Task ModificationFailed<T>(T original, T modified, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenModificationFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                await ((dynamic)listener).OnModificationFailedAsync(original, modified, context);
            }
        }

        public static async Task Removed<T>(T entity, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenRemoved<T>>();
            foreach (var listener in modifyListeners)
            {
                await ((dynamic)listener).OnRemovedAsync(entity, context);
            }
        }

        public static async Task RemoveFailed<T>(T entity, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenRemoveFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                await ((dynamic)listener).OnRemoveFailedAsync(entity, context);
            }
        }
    }
}
