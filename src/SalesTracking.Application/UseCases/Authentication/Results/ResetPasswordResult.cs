using SalesTracking.Application.Common.Results;

namespace SalesTracking.Application.UseCases.Authentication.Results
{
    public class ResetPasswordResult : MessageResult
    {
        public bool Succeeded { get; set; }
    }
}
