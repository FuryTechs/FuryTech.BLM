using System;
using System.Collections.Generic;
using System.Linq;

namespace BLM
{
    public static class Authorize
    {
        private static AuthorizationResult _elevatedResult = AuthorizationResult.Success("Elevated context");

        public static IQueryable<T> Collection<T>(IQueryable<T> entities, IContextInfo context) where T : class
        {
            if (ElevatedContext.IsElevated())
                return entities;

            var collectionAuthorizers = Loader.GetEntriesFor<IAuthorizeCollection<T>>();
            foreach (var collectionAuthorizer in collectionAuthorizers)
            {
                var auth = (IAuthorizeCollection<T>)collectionAuthorizer;
                entities = auth.AuthorizeCollection(entities, context);
            }

            return entities;
        }

        public static IEnumerable<AuthorizationResult> Create<T>(T entity, IContextInfo context)
        {
            if (ElevatedContext.IsElevated())
            {
                yield return _elevatedResult;
            }
            else
            {

                var createAuthorizers = Loader.GetEntriesFor<IAuthorizeCreate<T>>();
                foreach (var authorizer in createAuthorizers)
                {
                    var auth = (IAuthorizeCreate<T>) authorizer;
                    yield return auth.CanCreate(entity, context);
                }
            }
        }

        public static IEnumerable<AuthorizationResult> Modify<T>(T originalEntity, T modifiedEntity, IContextInfo context)
        {
            if (ElevatedContext.IsElevated())
            {
                yield return _elevatedResult;
            }
            else
            {

                var modifyAuthorizers = Loader.GetEntriesFor<IAuthorizeModify<T>>();
                foreach (var authorizer in modifyAuthorizers)
                {
                    var auth = (IAuthorizeModify<T>) authorizer;
                    yield return auth.CanModify(originalEntity, modifiedEntity, context);
                }
            }
        }

        public static IEnumerable<AuthorizationResult> Remove<T>(T entity, IContextInfo context)
        {
            if (ElevatedContext.IsElevated())
            {
                yield return _elevatedResult;
            }
            else
            {

                var removeAuthorizers = Loader.GetEntriesFor<IAuthorizeRemove<T>>();
                foreach (var authorizer in removeAuthorizers)
                {
                    var auth = (IAuthorizeRemove<T>) authorizer;
                    yield return auth.CanRemove(entity, context);
                }
            }
        }
    }
}
