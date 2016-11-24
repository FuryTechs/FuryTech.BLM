using System;

namespace BLM.Authorization
{
    public class AuthorizerNotFoundException : Exception
    {
        public AuthorizerNotFoundException(Type T) : base($"No IAuthorizer class found for '{T.FullName}'!")
        {
            
        }
    }
}
