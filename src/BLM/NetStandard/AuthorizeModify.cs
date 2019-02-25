using System.Threading.Tasks;
using FuryTechs.BLM.NetStandard.Interfaces;
using FuryTechs.BLM.NetStandard.Interfaces.Authorize;

namespace FuryTechs.BLM.NetStandard
{
    public abstract class AuthorizeModify<T> : IAuthorizeModify<T>
    {
        public abstract Task<AuthorizationResult> CanModifyAsync(T originalEntity, T modifiedEntity, IContextInfo ctx);
    }
}