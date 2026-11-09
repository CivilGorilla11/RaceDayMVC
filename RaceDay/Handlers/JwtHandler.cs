using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using RaceDay.Helpers;

namespace RaceDay.Handlers;

// DELEGATING HANDLER: sits between your service classes and the network, and runs on EVERY API call.
// It reads the token from session and adds the "Authorization: Bearer <token>" header,
// which is why the service classes never mention tokens.
public class JwtHandler : DelegatingHandler
{
    // IHttpContextAccessor is how a class outside a controller reaches the current request's session.
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetToken();

        // No token (guest browsing events) -> send the request without the header.
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);   // continue to the real network call
    }
}
