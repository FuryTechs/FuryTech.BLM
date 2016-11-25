using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching.Generic;
using System.Security.Principal;

namespace BLM.EventListeners
{
    public class EventListenerManager
    {
        private static EventListenerManager _instance;
        public static EventListenerManager Current => _instance ?? (_instance = new EventListenerManager());

        private readonly Dictionary<Type, IEventListener> _listeners = new Dictionary<Type, IEventListener>();

        private readonly MemoryCache<Type, IEnumerable<IEventListener>> _cache;
        private static object _cachelock;

        public IEventListener GetListener<T>()
        {
            return _listeners.FirstOrDefault(l => l.Key == typeof(T)).Value;
        }

        private void InitListeners()
        {
            var listenerTypes = BlmTypeLoader.GetLoadedTypes().Where(
                t => 
                typeof(IEventListener).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract
                ).ToList();
            foreach (var listenerType in listenerTypes)
            {
                var instance = Activator.CreateInstance(listenerType);
                _listeners.Add(listenerType, (IEventListener)instance);
            }
        }
        private EventListenerManager()
        {
            InitListeners();
            _cachelock = new object();
            _cache = new MemoryCache<Type, IEnumerable<IEventListener>>();
        }

        private IEnumerable<IEventListener> GetListenersForType(Type objType)
        {
            if (_cache.Contains(objType))
            {
                return _cache.Get(objType);
            }

            lock (_cachelock)
            {
                if (_cache.Contains(objType))
                {
                    return _cache.Get(objType);
                }

                var listenerType = typeof(IEventListener<>).MakeGenericType(objType);
                var filteredListeners = _listeners.Where(l => listenerType.IsAssignableFrom(l.Key)).Select(l => l.Value).ToList();

                foreach (var intr in objType.GetInterfaces())
                {
                    filteredListeners.AddRange(GetListenersForType(intr));
                }
               
                _cache.Add(objType, filteredListeners);
                return filteredListeners;
            }
        }

        private void TriggerMethod(Type objType, string methodName, object[] methodParams)
        {
            foreach (var listener in GetListenersForType(objType))
            {
                var methodInfo = listener.GetType().GetMethod(methodName);
                methodInfo.Invoke(listener, methodParams);
            }
        }




        public object TriggerOnBeforeCreate(object obj, IIdentity user)
        {
            var objType = obj.GetType();
            var methodName = "OnBeforeCreate";
            foreach (var listener in GetListenersForType(objType))
            {
                var methodInfo = listener.GetType().GetMethod(methodName);
                obj = methodInfo.Invoke(listener, new[] {obj, user});
            }

            return obj;
        }

        public object TriggerOnBeforeModify(object original, object modified, IIdentity user)
        {
            var objType = modified.GetType();
            var methodName = "OnBeforeModify";
            foreach (var listener in GetListenersForType(objType))
            {
                var methodInfo = listener.GetType().GetMethod(methodName);
                modified = methodInfo.Invoke(listener, new[] { original, modified, user });
            }

            return modified;
        }

        public void TriggerOnCreated(object obj, IIdentity user)
        {
            TriggerMethod(obj.GetType(), "OnCreated", new[] { obj, user });
        }

        public void TriggerOnCreationFailed(object obj, IIdentity user)
        {
            TriggerMethod(obj.GetType(), "OnCreationValidationFailed", new[] { obj, user });
        }

        public void TriggerOnModified(object originalObj, object modifiedObj, IIdentity user)
        {
            TriggerMethod(originalObj.GetType(), "OnModified", new[] { originalObj, modifiedObj, user });
        }

        public void TriggerOnModificationFailed(object originalObj, object modifiedObj, IIdentity user)
        {
            TriggerMethod(originalObj.GetType(), "OnModificationFailed", new[] { originalObj, modifiedObj, user });
        }

        public void TriggerOnRemoved(object removedObj, IIdentity user)
        {
            TriggerMethod(removedObj.GetType(), "OnRemoved", new[] { removedObj, user });
        }

        public void TriggerOnRemoveFailed(object removedObj, IIdentity user)
        {
            TriggerMethod(removedObj.GetType(), "OnRemoveFailed", new[] { removedObj, user });
        }

    }
}
