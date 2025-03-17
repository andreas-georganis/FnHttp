using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FnHttp;

public enum ContentAs
{
    String,
    ByteArray,
    Stream,
    Empty
}

public class FnHttpRequestHeaders: HttpHeaders
{
    public FnHttpRequestHeaders()
    {
        
    }
    
    public FnHttpRequestHeaders(HttpRequestHeaders headers)
    {
        foreach (var header in headers)
        {
            Add(header.Key, header.Value);
        }
    }
}

public class FnHttpRequest
{
    public FnHttpRequest()
    {
        
    }
    
    public FnHttpRequest(string? uri)
    {
        if (uri is null)
            return;
        Uri = new Uri(uri, UriKind.RelativeOrAbsolute);
    }
    
    
    private readonly object? _payload;

    public object? Payload
    {
        get => _payload;
        init
        {
            HttpContent = value switch
            {
                string s => new StringContent(s),
                byte[] b => new ByteArrayContent(b),
                Stream stream => new StreamContent(stream),
                HttpContent httpContent => httpContent,
                null => null,
                _ => JsonContent.Create(value, typeof(object), null, JsonSerializerOptions)
            };
            _payload = value;
        }
    }

    internal HttpContent? HttpContent { get; init; }
    
    public Uri? Uri { get; init; }
    
    public HttpMethod Method { get; init; } = HttpMethod.Get;
    
    public HttpCompletionOption CompletionOption { get; init; }
    
    /// <summary>
    /// How to read the response content
    /// </summary>
    public ContentAs Content { get; init; } = ContentAs.Stream;

    public int Timeout { get; init; } = System.Threading.Timeout.Infinite;
    
    public JsonSerializerOptions? JsonSerializerOptions { get; init; }
    
    public FnHttpRequestHeaders Headers { get; init; } = new();

    public HttpRequestMessage ToHttpRequestMessage()
    {
        var message = new HttpRequestMessage(Method, Uri)
        {
            Content = HttpContent
        };
        ApplyHeaders(message, Headers);
        return message;
    }
    
    private static void ApplyHeaders(HttpRequestMessage request, FnHttpRequestHeaders headers)
    {
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }
}

