using System;
using System.Linq;
using System.Runtime.CompilerServices;
using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Interpret;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("BLM.NetStandard.Tests")]
[assembly: InternalsVisibleTo("BLM.EntityFrameworkCore")]
[assembly: InternalsVisibleTo("BLM.EntityFrameworkCore.Tests")]

namespace FuryTech.BLM.NetStandard
{
    internal static class Interpret
    {
        public static T BeforeCreate<T>(T entity, IContextInfo context, IServiceProvider serviceProvider)
        {
            var createInterpreters = serviceProvider.GetServices<IBlmEntry>().OfType<IInterpretBeforeCreate<T, T>>();
            return createInterpreters.Cast<IInterpretBeforeCreate>().Aggregate(entity, (current, intr) => (T)intr.DoInterpret(current, context));
        }

        public static T BeforeModify<T>(T originalEntity, T modifiedEntity, IContextInfo context, IServiceProvider serviceProvider)
        {
            var modifyInterpreters = serviceProvider.GetServices<IBlmEntry>().OfType<IInterpretBeforeModify<T, T>>();
            return modifyInterpreters.Cast<IInterpretBeforeModify>().Aggregate(modifiedEntity, (current, intr) => (T)intr.DoInterpret(originalEntity, current, context));
        }
    }
}
