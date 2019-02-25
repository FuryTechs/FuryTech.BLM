using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTechs.BLM.NetStandard
{
    public abstract class AuthorizeCreate<T> : IAuthorizeCreate<T>
    {
        public abstract Task<AuthorizationResult> CanCreateAsync(T entity, IContextInfo ctx);
    }
}