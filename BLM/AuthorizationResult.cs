using System.Collections.Generic;

namespace BLM
{
    public class AuthorizationResult
    {
        private AuthorizationResult() { }

        public static AuthorizationResult Success()
        {
            return new AuthorizationResult()
            {
                HasSucceed = true
            };
        }

        public static AuthorizationResult Fail<T>(string message, T entity)
        {
            return new AuthorizationResult()
            {
                HasSucceed = false,
                Message = message
            };
        }

        public string Message { get; private set; }

        public bool HasSucceed { get; private set; }

        public List<AuthorizationResult> InnerResult = new List<AuthorizationResult>();
    }
}
