namespace FnHttp;

public static partial class FnHttpClientExtensions
{
    public static async ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError, Error>> PutAsync<T>(
        this FnHttpClient client, 
        string? requestUri = null,
        object? payload = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.SendAsync<T>(new FnHttpRequest(requestUri){Method = HttpMethod.Put, Payload = payload, CompletionOption = completionOption}, cancellationToken);
        
        return response;
    }
    
    public static async ValueTask<OneOf<T?, FnHttpRequestError, DeserializationError, Error>> PutAsync<T>(
        this FnHttpClient client, 
        Uri? requestUri = null,
        object? payload = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.SendAsync<T>(new FnHttpRequest{ Uri = requestUri ,Method = HttpMethod.Put,Payload = payload, CompletionOption = completionOption }, cancellationToken);
        
        return response;
    }
}