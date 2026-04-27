using ChatApp.Application.DTO;
using ChatApp.Web.Services.State;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

public class AuthorizationHandler : DelegatingHandler
{
    private readonly AppStateService _appState;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public AuthorizationHandler(AppStateService appState, IHttpClientFactory httpClientFactory)
    {
        _appState = appState;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var currentToken = _appState.Token;
        if (!string.IsNullOrEmpty(currentToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? validToken = null;

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (_appState.Token != currentToken)
                {
                    validToken = _appState.Token;
                }
                else
                {
                    validToken = await TryRefreshToken();
                }
            }
            finally
            {
                _semaphore.Release();
            }

            if (!string.IsNullOrEmpty(validToken))
            {
                using var retryRequest = await CloneHttpRequestMessageAsync(request, validToken);
                return await base.SendAsync(retryRequest, cancellationToken);
            }
        }

        return response;
    }

    private async Task<string?> TryRefreshToken()
    {
        try
        {
            var authClient = _httpClientFactory.CreateClient("AuthClient");
            var refreshResponse = await authClient.PostAsync("api/auth/refresh", null);

            if (refreshResponse.IsSuccessStatusCode)
            {
                var result = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
                if (result != null)
                {
                    await _appState.SetUserSessionAsync(result.User, result.AccessToken);
                    return result.AccessToken;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh failed: {ex.Message}");
        }

        // await _appState.Logout(); 
        return null;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request, string newToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        foreach (var header in request.Headers)
        {
            if (header.Key != "Authorization")
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        clone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var h in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
        }

        foreach (var option in request.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
        }

        return clone;
    }
}