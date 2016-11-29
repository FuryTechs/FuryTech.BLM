using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLM.Interfaces.Listen;

namespace BLM
{
    public class Listen
    {
        public static async Task Created<T>(T entity, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var createListeners = Loader.GetEntriesFor<IListenCreated<T>>();
                foreach (var createListener in createListeners)
                {
                    ((dynamic)createListener).OnCreatedAsync(entity, context);
                }
            });
        }

        public static async Task CreateFailed<T>(T entity, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var createFailListeners = Loader.GetEntriesFor<IListenCreateFailed<T>>();
                foreach (var listener in createFailListeners)
                {
                    ((dynamic)listener).OnCreateFailedAsync(entity, context);
                }
            });
        }

        public static async Task Modified<T>(T original, T modified, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var modifyListeners = Loader.GetEntriesFor<IListenModified<T>>();
                foreach (var listener in modifyListeners)
                {
                    ((dynamic)listener).OnModifiedAsync(original, modified, context);
                }
            });
        }

        public static async Task ModificationFailed<T>(T original, T modified, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var modifyListeners = Loader.GetEntriesFor<IListenModificationFailed<T>>();
                foreach (var listener in modifyListeners)
                {
                    ((dynamic)listener).OnModificationFailedAsync(original, modified, context);
                }
            });
        }

        public static async Task Removed<T>(T entity, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var modifyListeners = Loader.GetEntriesFor<IListenRemoved<T>>();
                foreach (var listener in modifyListeners)
                {
                    ((dynamic)listener).OnRemovedAsync(entity, context);
                }
            });
        }

        public static async Task RemoveFailed<T>(T entity, IContextInfo context)
        {
            await Task.Factory.StartNew(() =>
            {
                var modifyListeners = Loader.GetEntriesFor<IListenRemoveFailed<T>>();
                foreach (var listener in modifyListeners)
                {
                    ((dynamic)listener).OnRemoveFailedAsync(entity, context);
                }
            });
        }
    }
}
