using System.Collections.Generic;
using System.Linq;

namespace BLM
{
    public static class AuthoriaztionResultExtension
    {
        public static AuthorizationResult CreateAggregateResult(this IEnumerable<AuthorizationResult> results)
        {
            var resultList = results.ToList();
            if (resultList.Any(a => !a.HasSucceed))
            {
                var failResult = AuthorizationResult.Fail("");
                failResult.InnerResult.AddRange(resultList);
                return failResult;
            }
            var successResult = AuthorizationResult.Success();
            successResult.InnerResult.AddRange(resultList);

            return successResult;

        }
    }
}