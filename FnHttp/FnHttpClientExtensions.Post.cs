namespace FnHttp;

public static partial class FnHttpClientExtensions
{
    public static async ValueTask<OneOf<T?, FnHttpError, DeserializationError, Error>> PostAsync<T>(
        this FnHttpClient client, 
        string? requestUri = null,
        object? payload = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.Send<T>(new FnHttpRequest(requestUri){Method = HttpMethod.Post, Payload = payload, CompletionOption = completionOption}, cancellationToken);
        
        return response.IsT0? response.AsT0.Data.AsT0: response.AsT1 ;
    }
    
    public static async ValueTask<OneOf<T?, FnHttpError, DeserializationError, Error>> PostAsync<T>(
        this FnHttpClient client, 
        Uri? requestUri = null,
        object? payload = null,
        HttpCompletionOption completionOption = default,
        CancellationToken cancellationToken = default)
    {
        var response = await client.Send<T>(new FnHttpRequest{ Uri = requestUri ,Method = HttpMethod.Post,Payload = payload, CompletionOption = completionOption }, cancellationToken);
        
        return response.IsT0? response.AsT0.Data.AsT0: response.AsT1 ;
    }
}