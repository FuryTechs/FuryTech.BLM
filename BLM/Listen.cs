using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLM
{
    public class Listen
    {
        public static void Created<T>(T entity, IContextInfo context)
        {
            var createListeners = Loader.GetEntriesFor<IListenCreated<T>>();
            foreach (var createListener in createListeners)
            {
                ((IListenCreated<T>)createListener).OnCreated(entity, context);
            }
        }

        public static void CreateFailed<T>(T entity, IContextInfo context)
        {
            var createFailListeners = Loader.GetEntriesFor<IListenCreateFailed<T>>();
            foreach (var listener in createFailListeners)
            {
                ((IListenCreateFailed<T>)listener).OnCreateFailed(entity, context);
            }
        }

        public static void Modified<T>(T original, T modified, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenModified<T>>();
            foreach (var listener in modifyListeners)
            {
                ((IListenModified<T>)listener).OnModified(original, modified, context);
            }
        }

        public static void ModificationFailed<T>(T original, T modified, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenModificationFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                ((IListenModificationFailed<T>)listener).OnModificationFailed(original, modified, context);
            }
        }

        public static void Removed<T>(T entity, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenRemoved<T>>();
            foreach (var listener in modifyListeners)
            {
                ((IListenRemoved<T>)listener).OnRemoved(entity, context);
            }
        }

        public static void RemoveFailed<T>(T entity, IContextInfo context)
        {
            var modifyListeners = Loader.GetEntriesFor<IListenRemoveFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                ((IListenRemoveFailed<T>)listener).OnRemoveFailed(entity, context);
            }
        }
    }
}
