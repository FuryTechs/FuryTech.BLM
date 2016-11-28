using System;
using System.Collections.Generic;
using System.Linq;

namespace BLM
{
    public static class Authorize
    {
        public static IQueryable<T> Collection<T>(IQueryable<T> entities, IContextInfo context) where T : class
        {
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
            var createAuthorizers = Loader.GetEntriesFor<IAuthorizeCreate<T>>();
            foreach (var authorizer in createAuthorizers)
            {
                var auth = (IAuthorizeCreate<T>)authorizer;
                yield return auth.CanCreate(entity, context);
            }
        }

        public static IEnumerable<AuthorizationResult> Modify<T>(T originalEntity, T modifiedEntity, IContextInfo context)
        {
            var modifyAuthorizers = Loader.GetEntriesFor<IAuthorizeModify<T>>();
            foreach (var authorizer in modifyAuthorizers)
            {
                var auth = (IAuthorizeModify<T>)authorizer;
                yield return auth.CanModify(originalEntity, modifiedEntity, context);
            }
        }

        public static IEnumerable<AuthorizationResult> Remove<T>(T entity, IContextInfo context)
        {
            var removeAuthorizers = Loader.GetEntriesFor<IAuthorizeRemove<T>>();
            foreach (var authorizer in removeAuthorizers)
            {
                var auth = (IAuthorizeRemove<T>)authorizer;
                yield return auth.CanRemove(entity, context);
            }
        }
    }
}
