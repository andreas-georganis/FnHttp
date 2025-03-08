namespace FnHttp;
public interface IFnHttpClient
{
    internal IDeserializer DefaultDeserializer { get; }
    Aff<FnHttpResponse> SendAff(FnHttpRequest request, CancellationToken cancellationToken = default);
}

public class FnHttpClient : IFnHttpClient
{
    private readonly HttpClient _httpClient;
    
    public FnHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    IDeserializer IFnHttpClient.DefaultDeserializer { get; } = new SystemTextJsonDeserializer();
    
    public Aff<FnHttpResponse> SendAff(FnHttpRequest request, CancellationToken cancellationToken = default)
        => AffMaybe<FnHttpResponse>(async () =>
        {
            using var fRequest = request.ToHttpRequestMessage();

            using var cts = new CancellationTokenSource(request.Timeout); 
            using var lts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

            var response = await _httpClient.SendAsync(fRequest, request.CompletionOption, lts.Token).ConfigureAwait(false);
            
            var content = request.Content switch
            {
                Content.ByteArray => new RawContent(await response.Content.ReadAsByteArrayAsync(lts.Token).ConfigureAwait(false)),
                Content.Stream => new RawContent(await response.Content.ReadAsStreamAsync(lts.Token).ConfigureAwait(false)),
                Content.String => new RawContent(await response.Content.ReadAsStringAsync(lts.Token).ConfigureAwait(false)),
                _ => throw new ArgumentOutOfRangeException(nameof(request), "Unknown content type.")
            };
            
            var headers = response.Content.Headers;
            
            return new FnHttpResponse
            {
                HttpStatusCode = response.StatusCode,
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                CharSet = headers.ContentType?.CharSet,
                ContentType = headers.ContentType?.MediaType,
                ContentLength = headers.ContentLength,
                Content = content
            };
        });
}