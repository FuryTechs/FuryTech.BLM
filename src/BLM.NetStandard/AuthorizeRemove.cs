using System.Threading.Tasks;
using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTech.BLM.NetStandard
{
    public abstract class AuthorizeRemove<T> : IAuthorizeRemove<T>
    {
        public abstract Task<AuthorizationResult> CanRemoveAsync(T entity, IContextInfo ctx);
    }
}