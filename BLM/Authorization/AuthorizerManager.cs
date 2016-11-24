using System;
using System.Linq;

namespace BLM.Authorization
{
    public static class AuthorizerManager
    {
        public static IAuthorizer<T> GetAuthorizer<T>() where T : class
        {
            var entitytype = typeof(T);

            var authType = BlmTypeLoader.GetLoadedTypes().Where(t => typeof(IAuthorizer<>).MakeGenericType(entitytype).IsAssignableFrom(t)).ToList();
            if (!authType.Any())
            {
                throw new AuthorizerNotFoundException(entitytype);
            }
            if (authType.Count() > 1)
            {
                throw new AuthorizerMismatchException(entitytype);
            }
            {

                var instance = Activator.CreateInstance(authType.SingleOrDefault());
                return (IAuthorizer<T>) instance;
            }

        }
    }
}
