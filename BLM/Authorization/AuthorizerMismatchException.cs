using System;

namespace BLM.Authorization
{
    public class AuthorizerMismatchException : Exception
    {
        public AuthorizerMismatchException(Type T) : base($"Authorizer mismatch: multiple authorizers found for '{T.FullName}'!")
        {

        }
    }
}
