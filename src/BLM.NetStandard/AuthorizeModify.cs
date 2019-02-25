using System.Threading.Tasks;
using FuryTech.BLM.NetStandard.Interfaces;
using FuryTech.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTech.BLM.NetStandard
{
    public abstract class AuthorizeModify<T> : IAuthorizeModify<T>
    {
        public abstract Task<AuthorizationResult> CanModifyAsync(T originalEntity, T modifiedEntity, IContextInfo ctx);
    }
}