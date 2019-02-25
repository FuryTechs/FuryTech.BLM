using System.Threading.Tasks;
using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTech.BLM.NetStandard
{
    public abstract class AuthorizeCreate<T> : IAuthorizeCreate<T>
    {
        public abstract Task<AuthorizationResult> CanCreateAsync(T entity, IContextInfo ctx);
    }
}