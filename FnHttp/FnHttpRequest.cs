using System.Net.Http.Json;

namespace FnHttp;

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
    
    
    private object? _payload;

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
                _ => JsonContent.Create(@value)
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
    public Content Content { get; init; } = Content.Stream;

    public int Timeout { get; init; } = System.Threading.Timeout.Infinite;

    public HttpRequestMessage ToHttpRequestMessage()
    {
        return new HttpRequestMessage(Method, Uri)
        {
            Headers = {  },
            Content = HttpContent
        };
    }
}

public enum Content
{
    //Json,
    String,
    ByteArray,
    Stream
}