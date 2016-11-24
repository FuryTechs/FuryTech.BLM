using System;
using System.Collections.Generic;

namespace BLM
{
    public static class BlmTypeLoader
    {
        private static readonly object TypeLoaderLock = new object();
        private static List<Type> _loadedTypes;
        public static List<Type> GetLoadedTypes()
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
                            _loadedTypes.AddRange(assembly.GetTypes());
                        }
                    }
                }
            }
            return _loadedTypes;
        }
    }
}
