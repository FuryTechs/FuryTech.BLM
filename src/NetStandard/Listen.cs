﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Listen;
using Microsoft.Extensions.DependencyInjection;

namespace FuryTechs.BLM.NetStandard
{
    internal static class Listen
    {
        internal static async Task CreatedAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider
        )
        {
            var createListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenCreated<T>>();
            foreach (var createListener in createListeners)
            {
                await createListener.OnCreatedAsync(entity, context);
            }
        }

        internal static void Created<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            CreatedAsync(entity, context, serviceProvider).Wait();
        }

        internal static async Task CreateFailedAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider
            )
        {
            var createFailListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenCreateFailed<T>>();
            foreach (var listener in createFailListeners)
            {
                await listener.OnCreateFailedAsync(entity, context);
            }
        }

        internal static void CreateFailed<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            CreateFailedAsync(entity, context, serviceProvider).Wait();
        }

        internal static async Task ModifiedAsync<T>(
            T original,
            T modified,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {

            var modifyListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenModified<T>>();
            foreach (var listener in modifyListeners)
            {
                await listener.OnModifiedAsync(original, modified, context);
            }
        }

        internal static void Modified<T>(
            T original,
            T modified,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            ModifiedAsync(original, modified, context, serviceProvider).Wait();
        }

        internal static async Task ModificationFailedAsync<T>(
            T original,
            T modified,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            var modifyListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenModificationFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                await listener.OnModificationFailedAsync(original, modified, context);
            }
        }

        internal static void ModificationFailed<T>(
            T original,
            T modified,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            ModificationFailedAsync(original, modified, context, serviceProvider).Wait();
        }

        internal static async Task RemovedAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            var modifyListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenRemoved<T>>();
            foreach (var listener in modifyListeners)
            {
                await listener.OnRemovedAsync(entity, context);
            }
        }

        internal static void Removed<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            RemovedAsync(entity, context, serviceProvider).Wait();
        }

        internal static async Task RemoveFailedAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            var modifyListeners = serviceProvider.GetServices<IBlmEntry>().OfType<IListenRemoveFailed<T>>();
            foreach (var listener in modifyListeners)
            {
                await listener.OnRemoveFailedAsync(entity, context);
            }
        }

        internal static void RemoveFailed<T>(
            T entity,
            IContextInfo context,
            IServiceProvider serviceProvider)
        {
            RemoveFailedAsync(entity, context, serviceProvider).Wait();
        }
    }
}
