using System.Linq;
using System.Security.Principal;

namespace BLM
{
    public interface IContextInfo
    {
        IIdentity Identity { get; }
        IQueryable<T> GetFullEntitySet<T>() where T : class;
        IQueryable<T> GetAuthorizedEntitySet<T>() where T : class;
    }
}
