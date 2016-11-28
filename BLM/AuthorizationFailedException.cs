using System;

namespace BLM
{
    public class AuthorizationFailedException : UnauthorizedAccessException
    {
        public AuthorizationResult AuthorizationResult { get; }

        public AuthorizationFailedException(AuthorizationResult authResult)
        {
            AuthorizationResult = authResult;
        }
    }
}
