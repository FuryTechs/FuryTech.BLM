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

        public static IEnumerable<UnauthorizedAccessException> Create<T>(T entity, IContextInfo context)
        {
            var createAuthorizers = Loader.GetEntriesFor<IAuthorizeCreate<T>>();
            foreach (var authorizer in createAuthorizers)
            {
                var auth = (IAuthorizeCreate<T>)authorizer;
                if (!auth.CanCreate(entity, context))
                {
                    yield return new UnauthorizedAccessException($"Cannot create entity, authorizer: {auth.GetType().FullName}");
                }
            }
        }

        public static IEnumerable<UnauthorizedAccessException> Modify<T>(T originalEntity, T modifiedEntity, IContextInfo context)
        {
            var modifyAuthorizers = Loader.GetEntriesFor<IAuthorizeModify<T>>();
            foreach (var authorizer in modifyAuthorizers)
            {
                var auth = (IAuthorizeModify<T>)authorizer;
                if (!auth.CanModify(originalEntity, modifiedEntity, context))
                {
                    yield return new UnauthorizedAccessException($"Cannot modify entity, authorizer: {auth.GetType().FullName}");
                }
            }
        }

        public static IEnumerable<UnauthorizedAccessException> Remove<T>(T entity, IContextInfo context)
        {
            var removeAuthorizers = Loader.GetEntriesFor<IAuthorizeRemove<T>>();
            foreach (var authorizer in removeAuthorizers)
            {
                var auth = (IAuthorizeRemove<T>)authorizer;
                if (!auth.CanRemove(entity, context))
                {
                    yield return new UnauthorizedAccessException($"Cannot remove entity, authorizer: {auth.GetType().FullName}");
                }
            }
        }
    }
}
