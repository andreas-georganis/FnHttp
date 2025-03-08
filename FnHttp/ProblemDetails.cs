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
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
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