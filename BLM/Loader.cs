using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BLM
{
    public static class Loader
    {
        private static readonly object TypeLoaderLock = new object();
        private static List<Type> _loadedTypes;
        public static List<Type> Types
        {
            get
            {
                if (_loadedTypes == null)
                {
                    lock (TypeLoaderLock)
                    {
                        if (_loadedTypes == null)
                        {
                            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                            _loadedTypes = new List<Type>();
                            foreach (var assembly in assemblies)
                            {
                                _loadedTypes.AddRange(
                                    assembly.GetTypes().Where(a =>
                                        a.GetInterfaces().Contains(typeof(IBlmEntry))
                                        && a.IsClass
                                        && !a.IsAbstract
                                        ));
                            }
                        }
                    }
                }
                return _loadedTypes;
            }
        }


        private static readonly Dictionary<string, IBlmEntry> BlmInstances = new Dictionary<string, IBlmEntry>();

        public static T GetInstance<T>() where T : class, IBlmEntry, new()
        {
            var key = typeof(T).FullName;
            IBlmEntry currentInstance;
            if (BlmInstances.TryGetValue(key, out currentInstance))
            {
                return (T)currentInstance;
            }
            var instance = new T();
            BlmInstances.Add(key, instance);
            return instance;
        }

        private static readonly Dictionary<string, List<IBlmEntry>> EntriesByTypeCache = new Dictionary<string, List<IBlmEntry>>();


        private static List<Type> GetAllEntriesFor(Type entityType)
        {
            return Types.Where(t =>
                t.GetInterfaces().Any(intr => intr.GenericTypeArguments.Any(gt => gt.IsAssignableFrom(entityType))))
                .ToList();
        }

        public static List<IBlmEntry> GetEntriesFor<TBlmEntry>() where TBlmEntry : class, IBlmEntry
        {
            var key = typeof(TBlmEntry).FullName;

            List<IBlmEntry> entries;
            if (EntriesByTypeCache.TryGetValue(key, out entries))
            {
                return entries;
            }

            entries = new List<IBlmEntry>();
            var typesForEntity = GetAllEntriesFor(typeof(TBlmEntry).GetGenericArguments()[0]);
            //typesForEntity[1].GetInterfaces()[0].GetGenericTypeDefinition().IsAssignableFrom(typeof(TBlmEntry).GetGenericTypeDefinition())
            var typesForBlmEntry = typesForEntity.Where(t => t.GetInterfaces().Any(intr => intr.IsGenericType && typeof(TBlmEntry).GetGenericTypeDefinition().IsAssignableFrom(intr.GetGenericTypeDefinition())));

            foreach (var type in typesForBlmEntry)
            {
                //var isAssignable = typeof(TBlmEntry).IsAssignableFrom(type);

                //var hasAssignableInterface = type.GetInterfaces().Any(i => typeof(TBlmEntry).IsAssignableFrom(i));

                //if (isAssignable || hasAssignableInterface)
                //{
                TBlmEntry instance = (TBlmEntry)typeof(Loader).GetMethod("GetInstance").MakeGenericMethod(type).Invoke(null, null);
                entries.Add(instance);
                //}
            }

            EntriesByTypeCache.Add(key, entries);

            return entries;
        }
    }
}
