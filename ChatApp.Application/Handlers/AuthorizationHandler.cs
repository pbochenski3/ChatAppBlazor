
using ChatApp.Application.Interfaces;
using System.Net.Http.Headers;

namespace ChatApp.Application.Handlers
{
    public class AuthorizationHandler : System.Net.Http.DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;
        public AuthorizationHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null)
            {
                var token = await _tokenProvider.GetToken();
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
