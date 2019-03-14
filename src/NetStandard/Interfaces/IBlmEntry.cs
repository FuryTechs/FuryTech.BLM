using System.Collections.Generic;
using System.Linq;

namespace FuryTechs.BLM.NetStandard.Interfaces
{
    public interface IBlmEntry
    {
    }

    public static class Selector
    {
        public static IEnumerable<TAuthorizer> GetAuthorizers<TAuthorizer, TEntityType>(this IEnumerable<IBlmEntry> entries)
                where TAuthorizer : IBlmEntry
        {
            return entries.Where(x => x
                .GetType()
                .GetInterfaces()
                .Any(iFace =>
                    typeof(TAuthorizer).IsAssignableFrom(iFace)
                    ||
                    (
                        iFace.IsGenericType &&
                        iFace.GetGenericTypeDefinition() == typeof(TAuthorizer).GetGenericTypeDefinition() &&
                        iFace.GenericTypeArguments.Any(args => args.IsAssignableFrom(typeof(TEntityType)))
                    )
                )
            ).Select(x => (TAuthorizer)x);
        }
    }
}
