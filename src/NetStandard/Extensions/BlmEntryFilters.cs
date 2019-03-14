using FuryTechs.BLM.NetStandard.Exceptions;
using FuryTechs.BLM.NetStandard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuryTechs.BLM.NetStandard.Extensions
{
    internal static class BlmEntryFilters
    {

        internal static IEnumerable<IBlmEntry> GetBlmAuthorizers<TIn>(this IEnumerable<IBlmEntry> entries)
            where TIn : class, IBlmEntry
        {
            var authorizerType = typeof(TIn);
            var entityType = authorizerType.GenericTypeArguments[0];

            return entries.Where(blmEntry =>
                blmEntry
                .GetType()
                .GetInterfaces()
                .Any(iFace =>
                    iFace.IsGenericType &&
                    iFace.GetGenericTypeDefinition().IsAssignableFrom(typeof(TIn).GetGenericTypeDefinition()) &&
                    iFace.GenericTypeArguments[0].IsAssignableFrom(entityType)
                )
            );
        }

        //    internal static IEnumerable<IBlmEntry> GetAuthorizers<TIn, TOut>(this IEnumerable<IBlmEntry> entries)
        //       where TIn : class, IBlmEntry
        //    {
        //        var authorizerType = typeof(TIn);
        //        var entityType = authorizerType.GenericTypeArguments[0];

        //        return entries.Where(blmEntry =>
        //            blmEntry
        //            .GetType()
        //            .GetInterfaces()
        //            .Any(iFace =>
        //                iFace.IsGenericType &&
        //                iFace.GetGenericTypeDefinition().IsAssignableFrom(typeof(TIn).GetGenericTypeDefinition()) &&
        //                iFace.GenericTypeArguments[0].IsAssignableFrom(entityType)
        //            )
        //        );
        //    }
    }
}
