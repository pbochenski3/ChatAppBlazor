using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp.Web.Services.Common
{
    public class RetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            if (retryContext.PreviousRetryCount < 5)
            {
                return TimeSpan.FromSeconds(2);
            }
            if (retryContext.PreviousRetryCount < 10)
            {
                return TimeSpan.FromSeconds(10);
            }
            return TimeSpan.FromSeconds(30);
        }
    }
}
