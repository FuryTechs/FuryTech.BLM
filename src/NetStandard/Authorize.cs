using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Extensions;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Authorize;
using Microsoft.Extensions.DependencyInjection;

namespace FuryTechs.BLM.NetStandard
{
    internal static class Authorize
    {
        internal static async Task<IQueryable<T>> CollectionAsync<T>(
            IQueryable<T> entities,
            IContextInfo context,
            IServiceProvider providers
            ) where T : class
        {
            var collectionAuthorizers = providers.GetServices<IBlmEntry>().GetBlmAuthorizers<IAuthorizeCollection<T, T>>();
            foreach (var collectionAuthorizer in collectionAuthorizers)
            {
                entities = (await ((IAuthorizeCollection<T, T>)collectionAuthorizer).AuthorizeCollectionAsync(entities, context)).Cast<T>();
            }

            return entities;
        }

        internal static IQueryable<T> Collection<T>(
            IQueryable<T> entities,
            IContextInfo context,
            IServiceProvider providers
            ) where T : class
        {
            return CollectionAsync(entities, context, providers).Result;
        }

        internal static async Task<IEnumerable<AuthorizationResult>> CreateAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider providers)
        {
            var createAuthorizers = providers.GetServices<IBlmEntry>().GetBlmAuthorizers<IAuthorizeCreate<T>>();
            var results = new List<AuthorizationResult>();
            foreach (var authorizer in createAuthorizers)
            {
                results.Add(await ((IAuthorizeCreate<T>)authorizer).CanCreateAsync(entity, context));
            }
            return results;
        }

        internal static async Task<IEnumerable<AuthorizationResult>> ModifyAsync<T>(
            T originalEntity,
            T modifiedEntity,
            IContextInfo context,
            IServiceProvider providers)
        {
            var modifyAuthorizers = providers.GetServices<IBlmEntry>().GetBlmAuthorizers<IAuthorizeModify<T>>();
            var results = new List<AuthorizationResult>();

            foreach (var authorizer in modifyAuthorizers)
            {
                results.Add(await (authorizer as IAuthorizeModify<T>).CanModifyAsync(originalEntity, modifiedEntity, context));
            }
            return results;
        }

        internal static async Task<IEnumerable<AuthorizationResult>> RemoveAsync<T>(
            T entity,
            IContextInfo context,
            IServiceProvider providers)
        {
            var removeAuthorizers = providers.GetServices<IBlmEntry>().GetBlmAuthorizers<IAuthorizeRemove<T>>();

            var results = new List<AuthorizationResult>();

            foreach (var authorizer in removeAuthorizers)
            {
                results.Add(await ((IAuthorizeRemove<T>)authorizer).CanRemoveAsync(entity, context));
            }
            return results;
        }
    }
}
