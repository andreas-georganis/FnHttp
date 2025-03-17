using System.Net.Http.Json;
using System.Text.Json;

namespace FnHttp;

public class ProblemDetailsDelegatingHandler : DelegatingHandler
{
    public ProblemDetailsDelegatingHandler() : base(new HttpClientHandler()) { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var response = await base.SendAsync(request, ct);

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        if (mediaType != null && mediaType.Equals("application/problem+json", StringComparison.InvariantCultureIgnoreCase))
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>((JsonSerializerOptions?)null, ct) ?? new ProblemDetails();
            throw new ProblemDetailsException(problemDetails, response);
        }

        return response;
    }
}

public class ProblemDetails
{
    public string? Type { get; init; }
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
    public Dictionary<string, object>? Extensions { get; init; }
}

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException(ProblemDetails problemDetails, HttpResponseMessage response) 
        : base(problemDetails.Detail)
    {
        ProblemDetails = problemDetails;
        Response = response;
    }

    public ProblemDetails ProblemDetails { get; }
    public HttpResponseMessage Response { get; }
}